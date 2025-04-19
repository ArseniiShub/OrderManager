using OrderManager.Shared;
using OrderManager.Shared.Orders;

namespace OrderManager.WriteModel.Domain.Orders;

public interface IOrderValidator
{
    ValidationResult ValidateCreateOrder(string productName, string deliveryAddress);
    ValidationResult ValidateUpdateDeliveryAddress(Order order, string deliveryAddress);
    ValidationResult ValidateDispatch(Order order);
    ValidationResult ValidateOutForDelivery(Order order);
    ValidationResult ValidateDeliver(Order order);
    ValidationResult ValidateArchive(Order order);
    ValidationResult ValidateRestore(Order order);

}

public class OrderValidator : IOrderValidator
{
    public virtual ValidationResult ValidateCreateOrder(string productName, string deliveryAddress)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(productName))
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.ProductName)}.NotEmpty", "Product name is required");
        }

        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.DeliveryAddress)}.NotEmpty", "Delivery address is required");
        }

        return result;
    }

    private static readonly OrderStatus[] AllowedStatusesForUpdateDeliveryAddress = [OrderStatus.New];
    public virtual ValidationResult ValidateUpdateDeliveryAddress(Order order, string deliveryAddress)
    {
        var result = new ValidationResult();
        CheckArchived(result, order);

        if (!AllowedStatusesForUpdateDeliveryAddress.Contains(order.Status))
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.Status)}.InvalidStatusForUpdateDeliveryAddress",
                $"Order delivery address cannot be updated when in status {order.Status}. Allowed statuses are: {string.Join(", ", AllowedStatusesForUpdateDeliveryAddress)}");
        }

        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.DeliveryAddress)}.NotEmpty", "Delivery address is required");
        }

        return result;
    }

    public virtual ValidationResult ValidateDispatch(Order order)
    {
        var result = new ValidationResult();
        CheckArchived(result, order);

        if (order.Status != OrderStatus.New)
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.Status)}.InvalidTransition",
                $"Cannot transition from {order.Status} to {OrderStatus.Dispatched}");
        }

        return result;
    }

    public virtual ValidationResult ValidateOutForDelivery(Order order)
    {
        var result = new ValidationResult();
        CheckArchived(result, order);

        if (order.Status != OrderStatus.Dispatched)
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.Status)}.InvalidTransition",
                $"Cannot transition from {order.Status} to {OrderStatus.OutForDelivery}");
        }

        return result;
    }

    public virtual ValidationResult ValidateDeliver(Order order)
    {
        var result = new ValidationResult();
        CheckArchived(result, order);

        if (order.Status != OrderStatus.OutForDelivery)
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.Status)}.InvalidTransition",
                $"Cannot transition from {order.Status} to {OrderStatus.Delivered}");
        }

        return result;
    }

    private static readonly OrderStatus[] AllowedStatusesForArchive = [OrderStatus.New, OrderStatus.Delivered];
    public virtual ValidationResult ValidateArchive(Order order)
    {
        var result = new ValidationResult();
        
        if (order.IsArchived)
        {
            result.AddError($"{nameof(Order)}.AlreadyArchived",
                "Order is already archived");
        }

        if (!AllowedStatusesForArchive.Contains(order.Status))
        {
            result.AddError($"{nameof(Order)}.{nameof(Order.Status)}.InvalidStatusForArchive",
                $"Order cannot be archived when in status {order.Status}. Allowed statuses are: {string.Join(", ", AllowedStatusesForArchive)}");
        }

        return result;
    }

    public virtual ValidationResult ValidateRestore(Order order)
    {
        var result = new ValidationResult();
        
        if (!order.IsArchived)
        {
            result.AddError($"{nameof(Order)}.NotArchived",
                "Order is not archived");
        }

        return result;
    }
    
    private static void CheckArchived(ValidationResult result, Order order)
    {
        if (order.IsArchived)
        {
            result.AddError($"{nameof(Order)}.CannotModifyArchived", "Cannot modify an archived order");
        }
    }
}