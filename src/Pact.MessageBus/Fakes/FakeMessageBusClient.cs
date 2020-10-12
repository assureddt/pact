using System;
using RabbitMQ.Client;

namespace Pact.MessageBus.Fakes
{
    public class FakeMessageBusClient : IMessageBusClient, IDisposable
    {
        public ConnectionFactory ConnectionFactory { get; }
        public IConnection Connection { get; }
        public IModel Channel { get; }
        public MessageBusSettings Options { get; }
        public void SendError(byte[] data, string originalKey)
        {
        }

        public void Dispose()
        {
            
        }
    }
}