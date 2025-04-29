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
        foreach (var product in products)
        {
            if (await dbContext.Set<Product>().AnyAsync(x => x.Name == product.Name, ct))
            {
                continue;
            }
            try
            {
                dbContext.Set<Product>().Add(product);
                await dbContext.SaveChangesAsync(ct);
            }
            catch
            {
                // ignored
            }
        }
    }
}