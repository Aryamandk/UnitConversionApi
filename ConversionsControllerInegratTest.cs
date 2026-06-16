using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionApi.Models;

namespace UnitConversionApi.Tests;

public class ConversionsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostConvert_ValidRequest_Returns200WithResult()
    {
        var request = new ConversionRequest(100, "km", "mi");

        var response = await _client.PostAsJsonAsync("/api/conversions", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(62.1371, 0.001);
        result.Category.Should().Be("length");
    }

    [Fact]
    public async Task PostConvert_UnknownUnit_Returns422()
    {
        var request = new ConversionRequest(1, "unknown", "m");

        var response = await _client.PostAsJsonAsync("/api/conversions", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task PostConvert_CrossCategory_Returns422()
    {
        var request = new ConversionRequest(1, "m", "kg");

        var response = await _client.PostAsJsonAsync("/api/conversions", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetUnits_NoFilter_ReturnsAll()
    {
        var response = await _client.GetAsync("/api/conversions/units");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var units = await response.Content.ReadFromJsonAsync<List<UnitInfo>>();
        units.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetUnits_WithCategory_ReturnsFiltered()
    {
        var response = await _client.GetAsync("/api/conversions/units?category=length");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var units = await response.Content.ReadFromJsonAsync<List<UnitInfo>>();
        units.Should().NotBeNullOrEmpty();
        units!.Should().AllSatisfy(u => u.Category.Should().Be("length"));
    }

    [Fact]
    public async Task GetCategories_ReturnsNonEmptyList()
    {
        var response = await _client.GetAsync("/api/conversions/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var categories = await response.Content.ReadFromJsonAsync<List<string>>();
        categories.Should().Contain(["length", "temperature", "weight"]);
    }
}