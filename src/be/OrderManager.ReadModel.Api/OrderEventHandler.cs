using OrderManager.ReadModel.Api.Orders;
using OrderManager.Shared.Orders;
using OrderManager.Shared.Orders.Events;

namespace OrderManager.ReadModel.Api;

public static class OrderEventHandler
{
    public static async Task HandleAsync(OrderCreated orderCreated, IOrderRepository orderRepository)
    {
        var order = new Order
        {
            Id = orderCreated.Id,
            CreatedAt = orderCreated.DateTime,
            DeliveryAddress = orderCreated.DeliveryAddress,
            ProductName = orderCreated.ProductName,
            Status = OrderStatus.New
        };
        await CreateOrUpdate(order, orderRepository, updateLastModifiedAt: false);
    }

    public static async Task HandleAsync(OrderDeliveryAddressUpdated orderDeliveryAddressUpdated, IOrderRepository orderRepository)
    {
        var order = await GetOrder(orderDeliveryAddressUpdated.Id, orderRepository);
        order.DeliveryAddress = orderDeliveryAddressUpdated.DeliveryAddress;
        await CreateOrUpdate(order, orderRepository);
    }

    public static async Task HandleAsync(OrderDispatched orderDispatched, IOrderRepository orderRepository)
    {
        var order = await GetOrder(orderDispatched.Id, orderRepository);
        order.Status = OrderStatus.Dispatched;
        await CreateOrUpdate(order, orderRepository);
    }

    public static async Task HandleAsync(OrderOutForDelivery orderOutForDelivery, IOrderRepository orderRepository)
    {
        var order = await GetOrder(orderOutForDelivery.Id, orderRepository);
        order.Status = OrderStatus.OutForDelivery;
        await CreateOrUpdate(order, orderRepository);
    }

    public static async Task HandleAsync(OrderDelivered orderDelivered, IOrderRepository orderRepository)
    {
        var order = await GetOrder(orderDelivered.Id, orderRepository);
        order.Status = OrderStatus.Delivered;
        await CreateOrUpdate(order, orderRepository);
    }

    public static async Task HandleAsync(OrderArchived orderArchived, IOrderRepository orderRepository)
    {
        var order = await GetOrder(orderArchived.Id, orderRepository);
        order.IsArchived = true;
        await CreateOrUpdate(order, orderRepository);
    }

    public static async Task HandleAsync(OrderRestored orderRestored, IOrderRepository orderRepository)
    {
        var order = await GetOrder(orderRestored.Id, orderRepository);
        order.IsArchived = false;
        await CreateOrUpdate(order, orderRepository);
    }

    private static async Task<Order> GetOrder(Guid orderId, IOrderRepository orderRepository)
    {
        var order = await orderRepository.Get(orderId);
        if (order == null)
        {
            throw new InvalidOperationException($"Order with id '{orderId}' not found");
        }

        return order;
    }

    private static async Task CreateOrUpdate(Order order, IOrderRepository orderRepository, bool updateLastModifiedAt = true)
    {
        if (updateLastModifiedAt)
        {
            order.LastModifiedAt = DateTimeOffset.UtcNow;
        }

        await orderRepository.CreateOrUpdate(order);
    }
}