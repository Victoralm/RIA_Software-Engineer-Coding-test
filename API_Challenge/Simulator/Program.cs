using Serilog;

class Program
{
    static async Task WaitForApiAsync(string url, int timeoutSeconds = 30)
    {
        using var client = new HttpClient();
        var start = DateTime.Now;
        while ((DateTime.Now - start).TotalSeconds < timeoutSeconds)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch (Exception ex)
            {
                // Loga a exceção usando Serilog
                Log.Warning(ex, "Falha ao tentar conectar à API em {Url}", url);
            }
            await Task.Delay(5000);
        }
        throw new Exception("API não ficou disponível a tempo.");
    }

    static async Task Main(string[] args)
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Logs");
        Directory.CreateDirectory(logDirectory); // Garante que o diretório existe

        var logFile = Path.Combine(logDirectory, "simulator.log");

        // Configuração do Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            await WaitForApiAsync("https://localhost:7037/Customer/MaxId");

            var apiClient = new ApiClient("https://localhost:7037"); // ajuste a URL conforme necessário
            var generator = new CustomerGenerator();

            int nextId = await apiClient.GetMaxCustomerIdAsync() + 1;

            // Função auxiliar para logar no console e arquivo via Serilog
            void LogMessage(string message)
            {
                Log.Information(message);
            }

            // Envia várias requisições POST e GET em paralelo
            var tasks = new Task[5];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    var customers = generator.GenerateCustomers(2, ref nextId);
                    LogMessage($"[POST] Enviando {customers.Count} clientes: " +
                        string.Join(", ", customers.ConvertAll(c => $"\n{c.FirstName} {c.LastName} (Id={c.Id}, Age={c.Age})")));

                    try
                    {
                        var postResult = await apiClient.PostCustomersAsync(customers);
                        LogMessage($"[POST] Status: {postResult.StatusCode}, Conteúdo: {postResult.Content}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"[POST] Erro: {ex.Message}");
                    }

                    try
                    {
                        var (allCustomers, getStatus, getContent) = await apiClient.GetCustomersWithStatusAsync();
                        LogMessage($"[GET] Status: {getStatus}, Total customers: {allCustomers.Count}");
                        LogMessage($"[GET] Conteúdo: {getContent}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"[GET] Erro: {ex.Message}");
                    }
                });
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}