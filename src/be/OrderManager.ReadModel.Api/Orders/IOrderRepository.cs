namespace OrderManager.ReadModel.Api.Orders;

public interface IOrderRepository
{
    Task<Order?> Get(Guid orderId);
    Task CreateOrUpdate(Order order);
    Task<Order[]> GetAll();
}