using OrderManager.WriteModel.Application.Orders.Commands;

namespace OrderManager.WriteModel.Api.Orders.Requests;

public record CreateOrderRequest(string ProductName, string DeliveryAddress)
{
    public CreateOrderCommand ToCommand()
    {
        return new CreateOrderCommand(ProductName, DeliveryAddress);
    }
}