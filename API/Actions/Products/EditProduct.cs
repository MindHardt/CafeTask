using System.Text.Json;
using Api.Models;
using Data;
using Domain;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Actions.Products;

[Handler, MapPut("products/{ProductId:int}")]
public partial class EditProduct
{
    public record Request
    {
        [FromRoute]
        public required int ProductId { get; set; }
        [FromBody]
        public required Body Content { get; set; }
        
        public record Body
        {
            public required string Name { get; set; }
            public required decimal Price { get; set; }
        }
    }

    private static async ValueTask<Results<ValidationProblem, Ok<ProductModel>>> HandleAsync(
        [AsParameters] Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        var (problem, product) = await ValidateRequest(request, dataContext, ct);
        if (problem is not null)
        {
            return problem;
        }

        product.Name = request.Content.Name;
        product.Price = request.Content.Price;
        await dataContext.SaveChangesAsync(ct);

        return TypedResults.Ok(ProductModel.FromProduct(product));
    }
    
    private static async ValueTask<(ValidationProblem?, Product)> ValidateRequest(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        Dictionary<string, string[]> errors = [];
        Product? product = null;
        if (request.Content.Price <= 0)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Content.Price))] = ["NOT_POSITIVE"];
        }
        
        if (request.ProductId <= 0)
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.ProductId))] = ["NOT_FOUND"];
        }
        else
        {
            product = await dataContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, ct);
            if (product is null)
            {
                errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.ProductId))] = ["NOT_FOUND"];
            }
        }
        
        if (string.IsNullOrWhiteSpace(request.Content.Name))
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Content.Name))] = ["EMPTY"];
        }
        else if (await dataContext.Products.AnyAsync(x => x.Name == request.Content.Name, ct))
        {
            errors[JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Content.Name))] = ["NAME_TAKEN"];
        }

        return (errors.Count > 0 ? TypedResults.ValidationProblem(errors) : null, product!);
    }
}