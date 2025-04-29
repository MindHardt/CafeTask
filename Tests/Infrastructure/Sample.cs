using Api.Actions;
using Data;
using Domain;

namespace Tests.Infrastructure;

public static class Sample
{
    public static DateTimeOffset Date { get; } = 
        new(2025, 04, 29, 23, 50, 30, TimeSpan.Zero);
    
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
}