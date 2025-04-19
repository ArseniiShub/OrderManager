namespace OrderManager.Shared.Orders.Events;

public record OrderDispatched(Guid Id, DateTimeOffset DateTime) : Event(Id, DateTime);