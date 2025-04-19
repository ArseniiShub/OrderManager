namespace OrderManager.Shared.Orders.Events;

public record OrderDelivered(Guid Id, DateTimeOffset DateTime) : Event(Id, DateTime);