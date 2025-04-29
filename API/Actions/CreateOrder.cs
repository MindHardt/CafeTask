using System.Text.Json;
using Api.Models;
using Data;
using Domain;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Actions;

[Handler, MapPost("orders")]
public partial class CreateOrder
{
    public record Request
    {
        public required string CustomerName { get; set; }
        public required PaymentType PaymentType { get; set; }
        public required Dictionary<string, int> Products { get; set; }
    }

    private static async ValueTask<Results<Ok<OrderModel>, ValidationProblem>> HandleAsync(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        if (await ValidateRequest(request, dataContext, ct) is { } problem)
        {
            return problem;
        }

        var order = new Order
        {
            CustomerName = request.CustomerName,
            PaymentType = request.PaymentType,
            Products = request.Products
                .Select(x => new OrderProduct
                {
                    ProductName = x.Key,
                    Quantity = x.Value
                })
                .ToList(),
            Status = OrderStatus.InProgress,
            OrderDate = DateTimeOffset.UtcNow
        };
        dataContext.Orders.Add(order);
        await dataContext.SaveChangesAsync(ct);
        
        return TypedResults.Ok(OrderModel.FromOrder(order));
    }

    private static async Task<ValidationProblem?> ValidateRequest(
        Request request, 
        DataContext dataContext, 
        CancellationToken ct)
    {
        Dictionary<string, string[]> errors = [];

        if (Enum.IsDefined(request.PaymentType) is false)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.PaymentType))] = ["UNDEFINED"];
        }
        foreach (var (product, count) in request.Products)
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

        return errors.Count != 0 ? TypedResults.ValidationProblem(errors) : null;
    }
}