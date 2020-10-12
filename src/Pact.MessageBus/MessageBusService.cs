using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pact.MessageBus
{
    public class MessageBusService : IHostedService
    {
        public MessageBusService(ILogger<MessageBusService> logger, IServiceProvider services, IMessageBusClient client)
        {
            _logger = logger;
            _client = client;
            _services = services;
        }

        private readonly ILogger<MessageBusService> _logger;
        private readonly IMessageBusClient _client;
        private readonly IServiceProvider _services;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var services = _services.GetServices<IMessageBusListener>().ToList();

            _logger.LogTrace("Setting up message bus listeners. {count}", services.Count);

            foreach (var service in services)
            {
                _logger.LogTrace("Setting up listener. {name}", service.Name);
                await service.Setup(_client);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}