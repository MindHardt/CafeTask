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

    private static async ValueTask<Results<NotFound, Ok>> HandleAsync(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        if (request.ProductId <= 0)
        {
            return TypedResults.NotFound();
        }
        var product = await dataContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, ct);
        if (product is null)
        {
            return TypedResults.NotFound();
        }
        
        dataContext.Products.Remove(product);
        await dataContext.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}