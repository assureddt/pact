using RabbitMQ.Client;

namespace Pact.RabbitMQ
{
    public interface IMessageBusClient
    {
        IModel Channel { get; }
        MessageBusSettings Options { get; }
        void SendError(byte[] data, string originalKey);
    }
}
