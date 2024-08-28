namespace Ordering.Application.Orders.EventHandlers;

public class OrderCreatedEvenHandler : INotificationHandler<OrderUpdatedEvent>
{
    private readonly ILogger<OrderCreatedEvenHandler> _logger;

    public OrderCreatedEvenHandler(ILogger<OrderCreatedEvenHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain event handled: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}
