namespace Domain;

public record Order
{
    public int Id { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public PaymentType PaymentType { get; set; }
    public OrderStatus Status { get; set; }
    
    public ICollection<OrderProduct>? Products { get; set; }
}