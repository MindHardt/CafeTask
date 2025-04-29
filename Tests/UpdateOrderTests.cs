using System.Net.Http.Json;
using Api.Actions;
using Api.Models;
using Tests.Infrastructure;

namespace Tests;

public class UpdateOrderTests(ApiFixture fixture)
{
    [Fact]
    public async Task UpdateOrder_Success()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createRequest = Sample.CreateOrderRequest();
        var createResponse = await client.PostAsJsonAsync("/orders", createRequest, ApiFixture.JsonOptions, ct);
        createResponse.EnsureSuccessStatusCode();
        var order = await createResponse.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);

        var updateUri = "orders/" + order!.Id + "/products";
        var updateRequest = new UpdateOrder.Request.Body
        {
            Products = createRequest.Products.ToDictionary(
                x => x.Key,
                x => x.Value + 1)
        };
        var updateResponse = await client.PatchAsJsonAsync(updateUri, updateRequest, ApiFixture.JsonOptions, ct);
        updateResponse.EnsureSuccessStatusCode();
        var result = await updateResponse.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);
        
        Assert.NotNull(result);
        Assert.Equal(createRequest.CustomerName, result.CustomerName);
        Assert.Equal(createRequest.PaymentType, result.PaymentType);
        Assert.Equal(updateRequest.Products, result.Products.ToDictionary(), (l, r) 
            => l.Count == r.Count && l.All(kvp => r[kvp.Key] == kvp.Value));
    }

    public static TheoryData<UpdateOrder.Request.Body> ErrorBodies { get; } =
    [
        new UpdateOrder.Request.Body { Products = new Dictionary<string, int> { ["Ничего"] = 1 }},
        new UpdateOrder.Request.Body { Products = new Dictionary<string, int> { ["Тирамису"] = -1 }},
        new UpdateOrder.Request.Body { Products = new Dictionary<string, int> { ["Тирамису"] = 0 }}
    ];
    
    [Theory]
    [MemberData(nameof(ErrorBodies))]
    public async Task CreateOrder_Error(UpdateOrder.Request.Body body)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createRequest = Sample.CreateOrderRequest();
        var createResponse = await client.PostAsJsonAsync("/orders", createRequest, ApiFixture.JsonOptions, ct);
        createResponse.EnsureSuccessStatusCode();
        var order = await createResponse.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);

        var updateUri = "orders/" + order!.Id + "/products";
        var updateResponse = await client.PatchAsJsonAsync(updateUri, body, ApiFixture.JsonOptions, ct);
        Assert.False(updateResponse.IsSuccessStatusCode);
    }
}