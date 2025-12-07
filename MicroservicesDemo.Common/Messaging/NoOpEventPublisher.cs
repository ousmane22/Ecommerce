using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Ecommerce.Common.Messaging;

public class NoOpEventPublisher : IEventPublisher
{
    private readonly ILogger<NoOpEventPublisher> _logger;

    public NoOpEventPublisher(ILogger<NoOpEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T @event, string routingKey) where T : class
    {
        _logger.LogDebug("NoOpEventPublisher.PublishAsync called for routingKey {RoutingKey} with event {@Event}", routingKey, @event);
        return Task.CompletedTask;
    }
}
