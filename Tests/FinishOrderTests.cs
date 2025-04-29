using System.Net.Http.Json;
using Api.Actions;
using Api.Models;
using Domain;
using Tests.Infrastructure;

namespace Tests;

public class FinishOrderTests(ApiFixture fixture)
{
    [Theory]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Completed)]
    public async Task FinishOrder_Success(OrderStatus status)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createRes = await client.PostAsJsonAsync("orders", 
            Sample.CreateOrderRequest(), ApiFixture.JsonOptions, ct);
        createRes.EnsureSuccessStatusCode();
        var order = await createRes.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);

        var uri = "orders/" + order!.Id + "/finish";
        var body = new FinishOrder.Request.Body
        {
            Status = status,
        };
        var finishRes = await client.PatchAsJsonAsync(uri, body, ApiFixture.JsonOptions, ct);
        finishRes.EnsureSuccessStatusCode();
        var updatedOrder = await finishRes.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);
        
        Assert.Equal(status, updatedOrder!.Status);
    }
    
    [Theory]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Completed)]
    public async Task FinishOrder_OrderCompleted_Failure(OrderStatus status)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createRes = await client.PostAsJsonAsync("orders", 
            Sample.CreateOrderRequest(), ApiFixture.JsonOptions, ct);
        createRes.EnsureSuccessStatusCode();
        var order = await createRes.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);

        var uri = "orders/" + order!.Id + "/finish";
        var body = new FinishOrder.Request.Body
        {
            Status = status,
        };
        var finishRes = await client.PatchAsJsonAsync(uri, body, ApiFixture.JsonOptions, ct);
        finishRes.EnsureSuccessStatusCode();
        var finishResAgain = await client.PatchAsJsonAsync(uri, body, ApiFixture.JsonOptions, ct);
        
        Assert.False(finishResAgain.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task FinishOrder_BadStatus_Failure()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createRes = await client.PostAsJsonAsync("orders", 
            Sample.CreateOrderRequest(), ApiFixture.JsonOptions, ct);
        createRes.EnsureSuccessStatusCode();
        var order = await createRes.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);

        var uri = "orders/" + order!.Id + "/finish";
        var body = new FinishOrder.Request.Body
        {
            Status = OrderStatus.InProgress,
        };
        var finishRes = await client.PatchAsJsonAsync(uri, body, ApiFixture.JsonOptions, ct);
        
        Assert.False(finishRes.IsSuccessStatusCode);
    }
}