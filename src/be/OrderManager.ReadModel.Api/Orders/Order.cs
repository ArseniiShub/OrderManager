using OrderManager.Shared.Orders;

namespace OrderManager.ReadModel.Api.Orders;

public class Order
{
    public required Guid Id { get; init; }
    public required string ProductName { get; set; }
    public required string DeliveryAddress { get; set; }
    public required OrderStatus Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
    public bool IsArchived { get; set; }
}