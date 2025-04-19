namespace OrderManager.Shared.Orders.Events;

public record OrderRestored(Guid Id, DateTimeOffset DateTime) : Event(Id, DateTime);