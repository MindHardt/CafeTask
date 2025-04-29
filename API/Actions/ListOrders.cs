using System.Text.Json;
using Api.Models;
using Data;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Actions;

[Handler, MapGet("orders")]
public partial class ListOrders
{
    public record Request
    {
        public required DateTimeOffset From { get; set; }
        public required DateTimeOffset To { get; set; }
    }

    private static async ValueTask<Results<ValidationProblem, Ok<OrderModel[]>>> HandleAsync(
        [AsParameters] Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        if (ValidateRequest(request) is { } problem)
        {
            return problem;
        }

        var orders = await dataContext.Orders
            .Include(x => x.Products)
            .Where(x => request.To >= x.OrderDate && x.OrderDate >= request.From)
            .OrderBy(x => x.OrderDate)
            .ToArrayAsync(ct);
        
        return TypedResults.Ok(orders.Select(OrderModel.FromOrder).ToArray());
    }

    private static ValidationProblem? ValidateRequest(Request request)
    {
        return request.From < request.To
            ? null
            : TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(request.From))] = ["AFTER_TO"]
            });
    }
}