using System.Text.Json;
using Api.Models;
using Data;
using Domain;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Actions.Products;

[Handler, MapPost("products")]
public partial class CreateProduct
{
    public record Request
    {
        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }

    private static async ValueTask<Results<ValidationProblem, Ok<ProductModel>>> HandleAsync(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        var problem = await ValidateRequest(request, dataContext, ct);
        if (problem is not null)
        {
            return problem;
        }

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };
        dataContext.Products.Add(product);
        await dataContext.SaveChangesAsync(ct);
        
        return TypedResults.Ok(ProductModel.FromProduct(product));
    }

    private static async ValueTask<ValidationProblem?> ValidateRequest(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        Dictionary<string, string[]> errors = [];
        if (request.Price <= 0)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Price))] = ["NOT_POSITIVE"];
        }
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Name))] = ["EMPTY"];
        }
        else if (await dataContext.Products.AnyAsync(x => x.Name == request.Name, ct))
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Name))] = ["NAME_TAKEN"];
        }

        return errors.Count > 0 ? TypedResults.ValidationProblem(errors) : null;
    }
}