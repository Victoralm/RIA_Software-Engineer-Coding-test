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
                // Log the exception using Serilog
                Log.Warning(ex, "Failed to connect to API at {Url}", url);
            }
            await Task.Delay(5000);
        }
        throw new Exception("API was not available in time.");
    }

    static async Task Main(string[] args)
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Logs");
        Directory.CreateDirectory(logDirectory);

        var logFile = Path.Combine(logDirectory, "simulator.log");

        // Serilog configuration
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            await WaitForApiAsync("https://localhost:7037/Customer/MaxId");

            var apiClient = new ApiClient("https://localhost:7037");
            var generator = new CustomerGenerator();

            int nextId = await apiClient.GetMaxCustomerIdAsync() + 1;

            // Helper function to log to console and file via Serilog
            void LogMessage(string message)
            {
                Log.Information(message);
            }

            // Send multiple POST and GET requests in parallel
            var tasks = new Task[5];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    var customers = generator.GenerateCustomers(2, ref nextId);
                    LogMessage($"[POST] Sending {customers.Count} customers: " +
                        string.Join(", ", customers.ConvertAll(c => $"\n{c.FirstName} {c.LastName} (Id={c.Id}, Age={c.Age})")));

                    try
                    {
                        var postResult = await apiClient.PostCustomersAsync(customers);
                        LogMessage($"[POST] Status: {postResult.StatusCode}, Content: {postResult.Content}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"[POST] Error: {ex.Message}");
                    }

                    try
                    {
                        var (allCustomers, getStatus, getContent) = await apiClient.GetCustomersWithStatusAsync();
                        LogMessage($"[GET] Status: {getStatus}, Total customers: {allCustomers.Count}");
                        LogMessage($"[GET] Content: {getContent}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"[GET] Error: {ex.Message}");
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