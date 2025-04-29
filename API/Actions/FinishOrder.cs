using System.Text.Json;
using Api.Models;
using Data;
using Domain;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Actions;

[Handler, MapPatch("orders/{orderId:int}/finish")]
public partial class FinishOrder
{
    public record Request
    {
        [FromRoute]
        public required int OrderId { get; set; }
        [FromBody]
        public required Body Content { get; set; }

        public record Body
        {
            public required OrderStatus Status { get; set; }
        }
    }

    private static async ValueTask<Results<ValidationProblem, Ok<OrderModel>>> HandleAsync(
        [AsParameters] Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        var (validationProblem, order) = await ValidateRequest(request, dataContext, ct);
        if (validationProblem is not null)
        {
            return validationProblem;
        }
        
        order!.Status = request.Content.Status;
        await dataContext.SaveChangesAsync(ct);

        return TypedResults.Ok(OrderModel.FromOrder(order));
    }

    private static async Task<(ValidationProblem? problem, Order? order)> ValidateRequest(
        Request request, 
        DataContext dataContext,
        CancellationToken ct)
    {
        if (request.Content.Status is not (OrderStatus.Completed or OrderStatus.Cancelled))
        {
            return (TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Content.Status))] = ["BAD_STATUS"]
            }), null);
        }
        
        if (request.OrderId <= 0 ||
            await dataContext.Orders
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, ct) is not { } order)
        {
            return (TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(request.OrderId))] = ["ORDER_NOT_FOUND"]
            }), null);
        }
        
        if (order.Status is not OrderStatus.InProgress)
        {
            return (TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(request.OrderId))] = ["ORDER_NOT_ACTIVE"]
            }), order);
        }

        return (null, order);
    }
}