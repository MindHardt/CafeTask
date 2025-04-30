using Domain;

namespace Api.Models;

public record ProductModel(
    int Id,
    string Name,
    decimal Price)
{
    public static ProductModel FromProduct(Product product) => new(product.Id, product.Name, product.Price);
}