using System.ComponentModel;

namespace Api;

public static class Paginated
{
    public record Request
    {
        [DefaultValue(0)]
        public required int Skip { get; set; }
        [DefaultValue(20)]
        public required int Take { get; set; }

        public Response<T> CreateResponse<T>(IReadOnlyCollection<T> items, int? total) => new(
            items, total, Skip, Take);
    }

    public record Response<T>(
        IReadOnlyCollection<T> Items,
        int? Total, 
        int Skipped,
        int Taken);
    
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, Request request)
        => query.Skip(request.Skip).Take(request.Take);
}