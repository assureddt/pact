using RabbitMQ.Client;

namespace Pact.MessageBus
{
    public interface IMessageBusConnection
    {
        IModel GetModel();
    }
}