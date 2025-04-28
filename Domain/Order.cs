namespace Domain;

public record Order
{
    public int Id { get; set; }
    
    public required string CustomerName { get; set; }
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
    public required PaymentType PaymentType { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.InProgress;
    
    public ICollection<OrderProduct>? Products { get; set; }
}