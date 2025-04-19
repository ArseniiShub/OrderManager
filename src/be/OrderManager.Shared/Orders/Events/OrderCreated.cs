namespace OrderManager.Shared.Orders.Events;

public record OrderCreated(Guid Id, DateTimeOffset DateTime, string ProductName, string DeliveryAddress) : Event(Id, DateTime);