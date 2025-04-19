namespace OrderManager.Shared;

public record Event(Guid Id, DateTimeOffset DateTime)
{
    public Guid Id { get; init; } = Id;
    public DateTimeOffset DateTime { get; init; } = DateTime;
}