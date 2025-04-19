namespace OrderManager.WriteModel.Application.Orders.Commands;

public record CreateOrderCommand(string ProductName, string DeliveryAddress);