using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Pact.RabbitMQ.Fakes
{
    public class FakeMessageBusSender : IMessageBusSender
    {
        public FakeMessageBusSender(ILogger<MessageBusSender> logger, IMessageBusClient client)
        {
            Sent = new Dictionary<(string exchange, string key, Guid random), object>();
        }

        public Dictionary<(string exchange, string key, Guid random), object> Sent { get; set; }

        public void Send(object item, string exchange, string key)
        {
            Sent.Add((exchange, key, Guid.NewGuid()), item);
        }

        public Task<T> SendRPCAsync<T>(object item, string exchange, string key) where T : class
        {
            Sent.Add((exchange, key, Guid.NewGuid()), item);
            return Task.FromResult(default(T));
        }
    }
}