namespace Domain;

public record OrderProduct
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    public required string ProductName { get; set; }
    public Product? Product { get; set; }
    
    public int Quantity { get; set; }
}