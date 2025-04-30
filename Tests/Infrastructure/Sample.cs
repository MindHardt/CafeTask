using Api.Actions;
using Api.Actions.Products;
using Data;
using Domain;

namespace Tests.Infrastructure;

public static class Sample
{
    public static DateTimeOffset Date { get; } = 
        new(2025, 01, 01, 12, 00, 00, TimeSpan.Zero);
    
    public static CreateOrder.Request CreateOrderRequest() => new()
    {
        CustomerName = "Петров Пётр",
        PaymentType = PaymentType.CreditCard,
        Products = new Dictionary<string, int>
        {
            [Seeder.Products.Carbonara.Name] = 1,
            [Seeder.Products.Olivier.Name] = 2,
            [Seeder.Products.Tiramisu.Name] = 3
        }
    };

    private static volatile int _productNumber = 1;

    public static CreateProduct.Request CreateProductRequest() => new()
    {
        Name = "Тестовый продукт " + Interlocked.Add(ref _productNumber, 1),
        Price = 100
    };
}