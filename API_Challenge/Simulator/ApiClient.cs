using System.Net.Http.Json;

public class ApiClient
{
    private readonly HttpClient _client;

    public ApiClient(string baseUrl)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<int> GetMaxCustomerIdAsync()
    {
        var resp = await _client.GetAsync("/Customer/MaxId");
        resp.EnsureSuccessStatusCode();
        var str = await resp.Content.ReadAsStringAsync();
        return int.TryParse(str, out var id) ? id : 0;
    }

    public async Task<(System.Net.HttpStatusCode StatusCode, string Content)> PostCustomersAsync(List<CustomerDto> customers)
    {
        var payload = new { customers };
        var resp = await _client.PostAsJsonAsync("/Customer", payload);
        var content = await resp.Content.ReadAsStringAsync();
        return (resp.StatusCode, content);
    }

    public async Task<(List<CustomerDto> Customers, System.Net.HttpStatusCode StatusCode, string Content)> GetCustomersWithStatusAsync()
    {
        var resp = await _client.GetAsync("/Customer");
        var content = await resp.Content.ReadAsStringAsync();
        List<CustomerDto>? customers = null;
        try
        {
            customers = System.Text.Json.JsonSerializer.Deserialize<List<CustomerDto>>(content);
        }
        catch { customers = new List<CustomerDto>(); }
        return (customers ?? new List<CustomerDto>(), resp.StatusCode, content);
    }
}