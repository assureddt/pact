using RabbitMQ.Client;

namespace Pact.RabbitMQ.Fakes;

/// <summary>
/// A fake client used in unit tests.
/// </summary>
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