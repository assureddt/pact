using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Pact.RabbitMQ;

/// <summary>
/// The raw message bus connection handler
/// </summary>
public class MessageBusConnection : IMessageBusConnection, IDisposable
{
    public MessageBusConnection(ILogger<MessageBusConnection> logger, IOptions<MessageBusSettings> options)
    {
        _logger = logger;

        try
        {
            Options = options.Value;

            _logger.LogTrace("Connecting to message bus {host}", Options.Hostname);
            ConnectionFactory = new ConnectionFactory
            {
                HostName = Options.Hostname,
                UserName = Options.Username,
                Password = Options.Password,
                AutomaticRecoveryEnabled = true
            };
            Connection = ConnectionFactory.CreateConnection();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error connecting to message bus");
        }
    }

    private readonly ILogger<MessageBusConnection> _logger;
    public MessageBusSettings Options { get; }
    public ConnectionFactory ConnectionFactory { get; }
    public IConnection Connection { get; }

    /// <summary>
    /// Gets the current channel to the server.
    /// </summary>
    /// <returns></returns>
    public IModel GetModel()
    {
        try
        {
            var channel = Connection.CreateModel();

            //Set to each worker only gets one message at a time.
            channel.BasicQos(0, 1, false);

            return channel;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error creating channel");
            throw;
        }
    }

    public void Dispose()
    {
        _logger.LogDebug("Closing and shutting down message bus connection");
        Connection.Dispose();
    }

}