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

[Handler, MapPatch("orders/{orderId:int}/products")]
public partial class UpdateOrder
{
    public record Request
    {
        [FromRoute]
        public required int OrderId { get; set; }
        [FromBody]
        public required Body Content { get; set; }

        public record Body
        {
            public required Dictionary<string, int> Products { get; set; }
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
        
        dataContext.OrderProducts.RemoveRange(order!.Products!);
        order.Products = request.Content.Products
            .Select(kvp => new OrderProduct
            {
                OrderId = order.Id,
                ProductName = kvp.Key,
                Quantity = kvp.Value
            })
            .ToList();
        await dataContext.SaveChangesAsync(ct);

        return TypedResults.Ok(OrderModel.FromOrder(order));
    }
    
    private static async Task<(ValidationProblem? problem, Order? order)> ValidateRequest(
        Request request, 
        DataContext dataContext,
        CancellationToken ct)
    {
        Dictionary<string, string[]> errors = [];
        if (request.OrderId <= 0)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.OrderId))] = ["ORDER_NOT_FOUND"];
        }

        var order = await dataContext.Orders
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, ct);
        
        if (order is null)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.OrderId))] = ["ORDER_NOT_FOUND"];
        }
        else if (order.Status is not OrderStatus.InProgress)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.OrderId))] = ["ORDER_NOT_ACTIVE"];
        }
        
        foreach (var (product, count) in request.Content.Products)
        {
            List<string> productErrors = [];
            if (count <= 0)
            {
                productErrors.Add("COUNT_NOT_POSITIVE");
            }

            if (await dataContext.Products.AnyAsync(x => x.Name == product, ct) is false)
            {
                productErrors.Add("PRODUCT_NOT_FOUND");
            }

            if (productErrors.Count != 0)
            {
                errors[product] = [..productErrors];
            }
        }

        return errors.Count != 0 ? (TypedResults.ValidationProblem(errors), order) : (null, order);
    }
}