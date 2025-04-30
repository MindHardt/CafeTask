namespace Domain;

public record Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    
    public ICollection<OrderProduct>? Orders { get; set; } 
}