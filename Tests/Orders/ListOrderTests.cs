using System.Net.Http.Json;
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
        Sample.CreateOrderRequest(), Sample.CreateOrderRequest(), 
        Sample.CreateOrderRequest(), Sample.CreateOrderRequest()
    ];

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
    
    [Fact]
    public async Task ListOrders_Filter_Success()
    {
        var ct = TestContext.Current.CancellationToken;
        // ReSharper disable once LocalVariableHidesPrimaryConstructorParameter
        // this test needs to run in isolation
        await using var fixture = new ApiFixture();
        await fixture.InitializeAsync();
        var client = fixture.Api.CreateClient();

        List<OrderModel> orders = [];
        foreach (var req in CreateRequests)
        {
            var response = await client.PostAsJsonAsync("/orders", req, ApiFixture.JsonOptions, ct);
            var order = await response.Content.ReadFromJsonAsync<OrderModel>(ApiFixture.JsonOptions, ct);
            orders.Add(order!);
            // To ensure creation dates are not too close
            await Task.Delay(500, ct);
        }

        foreach (var (i, order) in orders.Index())
        {
            var uri = UriHelper.BuildRelative("/orders", query: 
                QueryString.Create(new Dictionary<string, string?>
                {
                    [nameof(ListOrders.Request.From)] = DateTimeOffset.UnixEpoch.ToString("O"),
                    [nameof(ListOrders.Request.To)] = order.OrderDate.ToString("O")
                }));

            var result = await client.GetFromJsonAsync<OrderModel[]>(uri, ApiFixture.JsonOptions, ct);
            Assert.Equal(i + 1, result!.Length);
        }
    }
}