using RabbitMQ.Client;

namespace Pact.RabbitMQ
{
    public interface IMessageBusConnection
    {
        IModel GetModel();
    }
}