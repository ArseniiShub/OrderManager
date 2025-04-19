using OrderManager.WriteModel.Application.Orders.Commands;

namespace OrderManager.WriteModel.Api.Orders.Requests;

public record UpdateOrderRequest(string DeliveryAddress)
{
    public UpdateOrderCommand ToCommand(Guid orderId)
    {
        return new UpdateOrderCommand(orderId, DeliveryAddress);
    }
}