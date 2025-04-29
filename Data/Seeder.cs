using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class Seeder
{
    public static class Products
    {
        public static Product Carbonara { get; } = new()
        {
            Name = "Паста карбонара",
            Price = 399
        };

        public static Product Olivier { get; } = new()
        {
            Name = "Салат оливье",
            Price = 249
        };

        public static Product Tiramisu { get; } = new()
        {
            Name = "Тирамису",
            Price = 299
        };
    }
    
    public static async Task SeedAsync(DbContext dbContext, bool schemaChanged, CancellationToken ct)
    {
        Product[] products = [Products.Carbonara, Products.Olivier, Products.Tiramisu];
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