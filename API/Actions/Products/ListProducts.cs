using Api.Models;
using Data;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Actions.Products;

[Handler, MapGet("products")]
public partial class ListProducts
{
    public record Request : Paginated.Request
    {
        public string? Query { get; set; }
    }

    private static async ValueTask<Ok<Paginated.Response<ProductModel>>> HandleAsync(
        Request request,
        DataContext dataContext,
        CancellationToken ct)
    {
        var query = dataContext.Products.AsQueryable();

        if (string.IsNullOrEmpty(request.Query) is false)
        {
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{request.Query}%"));
        }
        
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.Id)
            .Paginate(request)
            .Select(x => new ProductModel(x.Id, x.Name, x.Price))
            .ToListAsync(ct);
        
        return TypedResults.Ok(request.CreateResponse(items, total));
    }
}