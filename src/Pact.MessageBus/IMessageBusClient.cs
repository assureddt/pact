using RabbitMQ.Client;

namespace Pact.MessageBus
{
    public interface IMessageBusClient
    {
        IModel Channel { get; }
        MessageBusSettings Options { get; }
        void SendError(byte[] data, string originalKey);
    }
}
