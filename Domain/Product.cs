namespace Domain;

public record Product
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    
    public ICollection<OrderProduct>? Orders { get; set; } 
}