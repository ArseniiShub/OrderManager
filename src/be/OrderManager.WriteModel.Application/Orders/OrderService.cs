using Marten;
using OrderManager.WriteModel.Domain.Orders;
using OrderManager.Shared;
using OrderManager.Shared.Orders.Events;
using OrderManager.WriteModel.Application.Orders.Commands;
using Wolverine.Marten;

namespace OrderManager.WriteModel.Application.Orders;

public class OrderService
{
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly TimeProvider _timeProvider;

    public OrderService(IMartenOutbox outbox, IDocumentSession session, TimeProvider timeProvider)
    {
        _outbox = outbox;
        _session = session;
        _timeProvider = timeProvider;
    }

    public async Task<Result<Order>> CreateOrder(CreateOrderCommand command)
    {
        var order = new Order();
        var result = order.CreateOrder(_timeProvider.GetUtcNow(), command.ProductName, command.DeliveryAddress);
        if (result.IsFailure)
        {
            return Result.Failure<Order>(result.Error);
        }

        var orderCreated = result.Value;
        _session.Events.StartStream<Order>(order.Id, orderCreated);
        await _outbox.SendAsync(orderCreated);
        await _session.SaveChangesAsync();

        return order;
    }

    public async Task<Result> UpdateOrder(UpdateOrderCommand command)
    {
        return await ExecuteForOrder(command.OrderId,
            order => order.UpdateDeliveryAddress(_timeProvider.GetUtcNow(), command.DeliveryAddress));
    }

    public async Task<Result> Dispatch(DispatchCommand command)
    {
        return await ExecuteForOrder(command.OrderId, order => order.Dispatch(_timeProvider.GetUtcNow()));
    }

    public async Task<Result> OutForDelivery(OutForDeliveryCommand command)
    {
        return await ExecuteForOrder(command.OrderId, order => order.OutForDelivery(_timeProvider.GetUtcNow()));
    }

    public async Task<Result> Deliver(DeliverCommand command)
    {
        return await ExecuteForOrder(command.OrderId, order => order.Deliver(_timeProvider.GetUtcNow()));
    }

    public async Task<Result> Archive(ArchiveCommand command)
    {
        return await ExecuteForOrder(command.OrderId, order => order.Archive(_timeProvider.GetUtcNow()));
    }

    public async Task<Result> Restore(RestoreCommand command)
    {
        return await ExecuteForOrder(command.OrderId, order => order.Restore(_timeProvider.GetUtcNow()));
    }

    private async Task<Result> ExecuteForOrder<TEvent>(Guid orderId, Func<Order, Result<TEvent>> action)
        where TEvent : Event
    {
        var order = await _session.LoadAsync<Order>(orderId);

        if (order is null)
        {
            return Result.Failure(OrderErrors.NotFound(orderId));
        }

        var result = action(order);
        if (result.IsFailure)
        {
            return result.Error.Type == ErrorType.NoAction
                ? Result.Success()
                : Result.Failure(result.Error);
        }

        var @event = result.Value;

        _session.Events.Append(@event.Id, @event);
        await _outbox.SendAsync(@event);

        await _session.SaveChangesAsync();

        return Result.Success();
    }
}