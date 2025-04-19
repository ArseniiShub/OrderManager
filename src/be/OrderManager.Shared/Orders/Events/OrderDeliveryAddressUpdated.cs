namespace OrderManager.Shared.Orders.Events;

public record OrderDeliveryAddressUpdated(Guid Id, DateTimeOffset DateTime, string DeliveryAddress) : Event(Id, DateTime);