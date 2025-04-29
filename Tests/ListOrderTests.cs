using System.Net.Http.Json;
using System.Text.Json;
using Api;
using Api.Actions;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Tests.Infrastructure;

namespace Tests;

public class ListOrderTests(ApiFixture fixture)
{
    public static CreateOrder.Request[] CreateRequests { get; } =
    [
        Sample.CreateOrderRequest(), Sample.CreateOrderRequest(), Sample.CreateOrderRequest()
    ];
    
    [Fact]
    public async Task ListOrders_Success()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();
        
        foreach (var req in CreateRequests)
        {
            var response = await client.PostAsJsonAsync("/orders", req, ApiFixture.JsonOptions, ct);
            response.EnsureSuccessStatusCode();
        }

        var from = DateTimeOffset.UtcNow.AddHours(-1).ToString("O");
        var to = DateTimeOffset.UtcNow.AddHours(1).ToString("O");
        var uri = UriHelper.BuildRelative("/orders", query: 
            QueryString.Create(new Dictionary<string, string?>
            {
                [nameof(ListOrders.Request.From)] = from,
                [nameof(ListOrders.Request.To)] = to
            }));
        
        var orders = await client.GetFromJsonAsync<OrderModel[]>(uri, ApiFixture.JsonOptions, ct);
        Assert.NotNull(orders);
        Assert.Equal(orders.Length, CreateRequests.Length);
    }

    public static TheoryData<(DateTimeOffset? from, DateTimeOffset? to)> ErrorRanges { get; } =
    [
        (null, Sample.Date),
        (Sample.Date, null),
        (Sample.Date, Sample.Date.AddHours(-1))
    ];
    
    [Theory]
    [MemberData(nameof(ErrorRanges))]
    public async Task ListOrders_DateRangeError((DateTimeOffset? from, DateTimeOffset? to) range)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();
        
        var uri = UriHelper.BuildRelative("/orders", query: 
            QueryString.Create(new Dictionary<string, string?>
            {
                [nameof(ListOrders.Request.From)] = range.from?.AddHours(1).ToString("O"),
                [nameof(ListOrders.Request.To)] = range.to?.AddHours(1).ToString("O")
            }));
        
        var res = await client.GetAsync(uri, ct);
        Assert.False(res.IsSuccessStatusCode);
    }
}