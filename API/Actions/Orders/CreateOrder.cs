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
        var (problem, products) = await ValidateRequest(request, dataContext, ct);
        if (problem is not null)
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
                    Product = products[x.Key],
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

    private static async Task<(ValidationProblem?, Dictionary<string, Product>)> ValidateRequest(
        Request request, 
        DataContext dataContext, 
        CancellationToken ct)
    {
        Dictionary<string, string[]> errors = [];

        if (Enum.IsDefined(request.PaymentType) is false)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.PaymentType))] = ["UNDEFINED"];
        }
        Dictionary<string, Product> products = [];
        foreach (var (productName, count) in request.Products)
        {
            List<string> productErrors = [];
            if (count <= 0)
            {
                productErrors.Add("COUNT_NOT_POSITIVE");
            }

            var product = await dataContext.Products.FirstOrDefaultAsync(x => x.Name == productName, ct);
            if (product is null)
            {
                productErrors.Add("PRODUCT_NOT_FOUND");
            }
            else
            {
                products[productName] = product;
            }

            if (productErrors.Count != 0)
            {
                errors[productName] = [..productErrors];
            }
        }

        return (errors.Count != 0 ? TypedResults.ValidationProblem(errors) : null, products);
    }
}