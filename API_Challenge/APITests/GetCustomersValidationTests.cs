using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using static APITests.CustomerTestUtils;

namespace APITests;

public class GetCustomersValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GetCustomersValidationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomers_ShouldReturnAllCustomers_WhenCustomersExist()
    {
        int maxId = await GetMaxCustomerIdAsync(_client);

        // Adiciona clientes via POST
        var customersToAdd = new[]
        {
            new { id = maxId + 1, firstName = "Anna", lastName = "Brown", age = 22 },
            new { id = maxId + 2, firstName = "Zack", lastName = "Smith", age = 35 }
        };
        var postResponse = await _client.PostAsJsonAsync("/Customer", new { customers = customersToAdd });
        var content = await postResponse.Content.ReadAsStringAsync();
        postResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Faz o GET
        var response = await _client.GetAsync("/Customer");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().NotBeNullOrEmpty();
        customers.Should().ContainSingle(c => c.Id == maxId + 1 && c.FirstName == "Anna" && c.LastName == "Brown" && c.Age == 22);
        customers.Should().ContainSingle(c => c.Id == maxId + 2 && c.FirstName == "Zack" && c.LastName == "Smith" && c.Age == 35);
    }

    // DTO auxiliar para deserialização
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
    }


}