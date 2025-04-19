namespace OrderManager.WriteModel.Application.Orders.Commands;

public record UpdateOrderCommand(Guid OrderId, string DeliveryAddress);