namespace Domain;

public enum OrderStatus : sbyte
{
    Cancelled = -1,
    InProgress = 0,
    Completed = 1,
}