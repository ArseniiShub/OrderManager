namespace OrderManager.Shared.Orders.Events;

public record OrderOutForDelivery(Guid Id, DateTimeOffset DateTime) : Event(Id, DateTime);