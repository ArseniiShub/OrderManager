using OrderManager.Shared;
using OrderManager.Shared.Orders;

namespace OrderManager.WriteModel.Domain.Orders;

public static class OrderErrors
{
    public static Error NotFound(Guid orderId) =>
        Error.NotFound("Orders.NotFound", $"The order with the Id = '{orderId}' was not found");

    public static Error InvalidStatusTransition(Guid id, OrderStatus currentStatus, OrderStatus transitionStatus) =>
        Error.Failure("Orders.InvalidStatusTransition",
            $"The order with the Id = '{id}' cannot be transitioned from '{currentStatus}' to '{transitionStatus}'");
}