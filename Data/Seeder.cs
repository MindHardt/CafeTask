using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class Seeder
{
    public static async Task SeedAsync(DbContext dbContext, bool schemaChanged, CancellationToken ct)
    {
        Product[] products =
        [
            new()
            {
                Name = "Паста карбонара",
                Price = 399
            },
            new()
            {
                Name = "Салат оливье",
                Price = 249
            },
            new()
            {
                Name = "Тирамису",
                Price = 299
            }
        ];
        dbContext.Set<Product>().AddRange(products);
        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch
        {
            // ignored
        }
    }
}