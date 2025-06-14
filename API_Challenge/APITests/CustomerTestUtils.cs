namespace APITests;

public static class CustomerTestUtils
{
    public static async Task<int> GetMaxCustomerIdAsync(HttpClient client)
    {
        var response = await client.GetAsync("/Customer/MaxId");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return int.Parse(content);
    }
}