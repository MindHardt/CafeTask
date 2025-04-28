using Domain;

namespace Api.Models;

public record OrderModel(
    int Id,
    string CustomerName,
    DateTimeOffset OrderDate,
    PaymentType PaymentType,
    OrderStatus OrderStatus,
    IReadOnlyDictionary<string, int> Products,
    decimal TotalPrice);