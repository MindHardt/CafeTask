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
        Dictionary<string, string[]> productErrors = [];
        foreach (var (product, count) in request.Products)
        {
            List<string> errors = [];
            if (count <= 0)
            {
                errors.Add("COUNT_NOT_POSITIVE");
            }

            if (await dataContext.Products.AnyAsync(x => x.Name == product, ct) is false)
            {
                errors.Add("PRODUCT_NOT_FOUND");
            }

            if (errors.Count != 0)
            {
                productErrors[product] = [..errors];
            }
        }

        return productErrors.Count != 0 ? TypedResults.ValidationProblem(productErrors) : null;
    }
}