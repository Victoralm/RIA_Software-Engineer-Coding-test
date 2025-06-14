using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using static APITests.CustomerTestUtils;

namespace APITests;

public class PostCustomersValidationTests : IClassFixture<WebApplicationFactory<Program>> 
{
    private readonly HttpClient _client;

    public PostCustomersValidationTests(WebApplicationFactory<Program> factory) 
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostCustomers_ShouldReturnOk_WhenAllCustomersAreValid()
    {
        int maxId = await GetMaxCustomerIdAsync(_client);

        var customers = new[]
        {
            new { id = maxId + 1, firstName = "John", lastName = "Doe", age = 25 },
            new { id = maxId + 2, firstName = "Jane", lastName = "Smith", age = 30 }
        };

        var response = await _client.PostAsJsonAsync("/Customer", new { customers });
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostCustomers_ShouldReturnBadRequest_WhenAnyFieldIsMissing()
    {
        int maxId = await GetMaxCustomerIdAsync(_client);

        var customers = new[]
        {
            new { id = maxId + 1, firstName = "John", lastName = "Doe" } // age missing
        };

        var response = await _client.PostAsJsonAsync("/Customer", new { customers });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Age is required");
    }

    [Fact]
    public async Task PostCustomers_ShouldReturnBadRequest_WhenAgeIsBelow18()
    {
        int maxId = await GetMaxCustomerIdAsync(_client);

        var customers = new[]
        {
            new { id = maxId + 1, firstName = "John", lastName = "Doe", age = 17 }
        };

        var response = await _client.PostAsJsonAsync("/Customer", new { customers });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Age must be greater than 18");
    }

    [Fact]
    public async Task PostCustomers_ShouldReturnBadRequest_WhenIdIsDuplicated()
    {
        int maxId = await GetMaxCustomerIdAsync(_client);

        // Primeiro, adiciona um cliente válido
        var customers1 = new[]
        {
            new { id = maxId + 1, firstName = "John", lastName = "Doe", age = 25 }
        };

        var response1 = await _client.PostAsJsonAsync("/Customer", new { customers = customers1 });
        var content = await response1.Content.ReadAsStringAsync();

        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Agora, tenta adicionar outro cliente com o mesmo ID
        var customers2 = new[]
        {
            new { id = maxId + 1, firstName = "Jane", lastName = "Smith", age = 30 }
        };
        var response2 = await _client.PostAsJsonAsync("/Customer", new { customers = customers2 });

        response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        content = await response2.Content.ReadAsStringAsync();
        content.Should().Contain("The given Id already exist");
    }

    [Fact]
    public async Task PostCustomers_ShouldReturnBadRequest_WhenAnyCustomerIsInvalid()
    {
        int maxId = await GetMaxCustomerIdAsync(_client);

        var customers = new[]
        {
            new { id = maxId + 1, firstName = "John", lastName = "Doe", age = 25 },
            new { id = maxId + 2, firstName = "", lastName = "Smith", age = 30 } // firstName vazio
        };

        var response = await _client.PostAsJsonAsync("/Customer", new { customers });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("FirstName is required");
    }
}