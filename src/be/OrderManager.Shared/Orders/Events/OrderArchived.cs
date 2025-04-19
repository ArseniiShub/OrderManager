namespace OrderManager.Shared.Orders.Events;

public record OrderArchived(Guid Id, DateTimeOffset DateTime) : Event(Id, DateTime);