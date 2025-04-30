using System.Text.Json;
using Data;
using Domain;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Actions.Products;

[Handler, MapDelete("products/{ProductId:int}")]
public partial class DeleteProduct
{
    public record Request
    {
        public required int ProductId { get; set; }
    }

    private static async ValueTask<Results<ValidationProblem, Ok>> HandleAsync(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        var (problem, product) = await ValidateRequest(request, dataContext, ct);
        if (problem is not null)
        {
            return problem;
        }
        
        dataContext.Products.Remove(product);
        await dataContext.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }

    private static async ValueTask<(ValidationProblem?, Product)> ValidateRequest(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        if (request.ProductId <= 0)
        {
            return (TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(request.ProductId))] = ["NOT_FOUND"]
            }), null!);
        }
        var product = await dataContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, ct);
        if (product is null)
        {
            return (TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [JsonNamingPolicy.CamelCase.ConvertName(nameof(request.ProductId))] = ["NOT_FOUND"]
            }), null!);
        }

        return (null, product);
    }
}