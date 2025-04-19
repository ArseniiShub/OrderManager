using OrderManager.Shared;
using OrderManager.Shared.Orders;
using OrderManager.Shared.Orders.Events;

namespace OrderManager.WriteModel.Domain.Orders;

public class Order
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public bool IsArchived { get; set; }

    private static readonly IOrderValidator OrderValidator = new OrderValidator();
    public virtual IOrderValidator GetValidator() => OrderValidator;

    public Result<OrderCreated> CreateOrder(DateTimeOffset dateTime, string productName, string deliveryAddress)
    {
        var validationResult = GetValidator().ValidateCreateOrder(productName, deliveryAddress);
        if (validationResult.IsNotValid)
        {
            return Result.Failure<OrderCreated>(ValidationError.FromValidationResult(validationResult));
        }

        var orderCreated = new OrderCreated(Guid.CreateVersion7(), dateTime, productName, deliveryAddress);
        Apply(orderCreated);

        return Result.Success(orderCreated);
    }

    public Result<OrderDeliveryAddressUpdated> UpdateDeliveryAddress(DateTimeOffset dateTime, string deliveryAddress)
    {
        if (DeliveryAddress == deliveryAddress)
        {
            return Result.Failure<OrderDeliveryAddressUpdated>(Error.NoAction);
        }

        var validationResult = GetValidator().ValidateUpdateDeliveryAddress(this, deliveryAddress);
        if (validationResult.IsNotValid)
        {
            return Result.Failure<OrderDeliveryAddressUpdated>(ValidationError.FromValidationResult(validationResult));
        }

        var orderDeliveryAddressUpdated = new OrderDeliveryAddressUpdated(Id, dateTime, deliveryAddress);
        Apply(orderDeliveryAddressUpdated);

        return Result.Success(orderDeliveryAddressUpdated);
    }

    public Result<OrderDispatched> Dispatch(DateTimeOffset dateTime)
    {
        var validationResult = GetValidator().ValidateDispatch(this);
        if (validationResult.IsNotValid)
        {
            return Result.Failure<OrderDispatched>(ValidationError.FromValidationResult(validationResult));
        }

        var orderDispatched = new OrderDispatched(Id, dateTime);
        Apply(orderDispatched);

        return Result.Success(orderDispatched);
    }

    public Result<OrderOutForDelivery> OutForDelivery(DateTimeOffset dateTime)
    {
        var validationResult = GetValidator().ValidateOutForDelivery(this);
        if (validationResult.IsNotValid)
        {
            return Result.Failure<OrderOutForDelivery>(ValidationError.FromValidationResult(validationResult));
        }

        var orderOutForDelivery = new OrderOutForDelivery(Id, dateTime);
        Apply(orderOutForDelivery);

        return Result.Success(orderOutForDelivery);
    }

    public Result<OrderDelivered> Deliver(DateTimeOffset dateTime)
    {
        var validationResult = GetValidator().ValidateDeliver(this);
        if (validationResult.IsNotValid)
        {
            return Result.Failure<OrderDelivered>(ValidationError.FromValidationResult(validationResult));
        }

        var orderDelivered = new OrderDelivered(Id, dateTime);
        Apply(orderDelivered);

        return Result.Success(orderDelivered);
    }

    public Result<OrderArchived> Archive(DateTimeOffset dateTime)
    {
        var validationResult = GetValidator().ValidateArchive(this);
        if (validationResult.IsNotValid)
        {
            return Result.Failure<OrderArchived>(ValidationError.FromValidationResult(validationResult));
        }

        var orderArchived = new OrderArchived(Id, dateTime);
        Apply(orderArchived);

        return Result.Success(orderArchived);
    }

    public Result<OrderRestored> Restore(DateTimeOffset dateTime)
    {
        var validationResult = GetValidator().ValidateRestore(this);
        if (validationResult.IsNotValid)
        {
            return Result.Failure<OrderRestored>(ValidationError.FromValidationResult(validationResult));
        }

        var orderRestored = new OrderRestored(Id, dateTime);
        Apply(orderRestored);

        return Result.Success(orderRestored);
    }


    public void Apply(OrderCreated @event)
    {
        Id = @event.Id;
        ProductName = @event.ProductName;
        DeliveryAddress = @event.DeliveryAddress;
        Status = OrderStatus.New;
    }

    public void Apply(OrderDeliveryAddressUpdated @event)
    {
        DeliveryAddress = @event.DeliveryAddress;
    }

    public void Apply(OrderDispatched @event)
    {
        Status = OrderStatus.Dispatched;
    }

    public void Apply(OrderOutForDelivery @event)
    {
        Status = OrderStatus.OutForDelivery;
    }

    public void Apply(OrderDelivered @event)
    {
        Status = OrderStatus.Delivered;
    }

    public void Apply(OrderArchived @event)
    {
        IsArchived = true;
    }

    public void Apply(OrderRestored @event)
    {
        IsArchived = false;
    }
}