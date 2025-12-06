namespace Ecommerce.Common.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, string routingKey) where T : class;
}