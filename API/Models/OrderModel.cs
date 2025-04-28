using Domain;

namespace Api.Models;

public record OrderModel(
    int Id,
    string CustomerName,
    DateTimeOffset OrderDate,
    PaymentType PaymentType,
    OrderStatus Status,
    IReadOnlyDictionary<string, int> Products)
{
    public static OrderModel FromOrder(Order order) => new(
        order.Id,
        order.CustomerName,
        order.OrderDate,
        order.PaymentType,
        order.Status,
        order.Products!.ToDictionary(x => x.ProductName, x => x.Quantity));
}