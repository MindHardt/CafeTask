using System.Net.Http.Json;
using System.Text.Json;
using Api;
using Api.Actions;
using Api.Models;
using Data;
using Domain;
using Tests.Infrastructure;

namespace Tests;

public class CreateOrderTests(ApiFixture fixture)
{
    [Fact]
    public async Task CreateOrder_Success()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var request = Sample.CreateOrderRequest();
        var response = await client.PostAsJsonAsync("/orders", request, ApiFixture.JsonOptions, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);
        
        Assert.NotNull(result);
        Assert.Equal(request.CustomerName, result.CustomerName);
        Assert.Equal(request.PaymentType, result.PaymentType);
        Assert.Equal(request.Products, result.Products.ToDictionary(), (l, r) 
            => l.Count == r.Count && l.All(kvp => r[kvp.Key] == kvp.Value));
    }

    public static TheoryData<CreateOrder.Request> ErrorRequests { get; } =
    [
        Sample.CreateOrderRequest() with { PaymentType = 0 },
        Sample.CreateOrderRequest() with { Products = null! },
        Sample.CreateOrderRequest() with { CustomerName = null! },
        Sample.CreateOrderRequest() with { Products = new Dictionary<string, int> { ["Ничего"] = 1 }},
        Sample.CreateOrderRequest() with { Products = new Dictionary<string, int> { ["Тирамису"] = -1 }},
        Sample.CreateOrderRequest() with { Products = new Dictionary<string, int> { ["Тирамису"] = 0 }}
    ];
    
    [Theory]
    [MemberData(nameof(ErrorRequests))]
    public async Task CreateOrder_Error(CreateOrder.Request request)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();
        
        var response = await client.PostAsJsonAsync("/orders", request, ApiFixture.JsonOptions, ct);
        
        Assert.False(response.IsSuccessStatusCode);
    }
}