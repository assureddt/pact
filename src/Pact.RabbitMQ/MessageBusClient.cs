using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Pact.RabbitMQ;

/// <summary>
/// A raw message bus client object.
/// </summary>
public class MessageBusClient : IMessageBusClient, IDisposable
{
    public MessageBusClient(ILogger<MessageBusClient> logger, IOptions<MessageBusSettings> options, IMessageBusConnection connection)
    {
        _logger = logger;

        try
        {
            Options = options.Value;

            Channel = connection.GetModel();

            //Set to each worker only gets one message at a time.
            Channel.BasicQos(0, 1, false);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error connecting to message bus");
            return;
        }

        //Setup Exchanges
        try
        {
            if (Options.Exchanges != null)
            {
                foreach (var exchange in Options.Exchanges)
                {
                    _logger.LogTrace("Setting up exchange {name}", exchange.Name);
                    Channel.ExchangeDeclare(exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete);
                }
            }

            if (Options.ErrorExchange != null)
            {
                _logger.LogTrace("Setting up error exchange {name}", Options.ErrorExchange);

                //Error exchange
                Channel.ExchangeDeclare(Options.ErrorExchange, "topic", true);

                //Error queue
                Channel.QueueDeclare(Options.ErrorExchange, true, false, false);

                //Bind it to the exchange
                Channel.QueueBind(Options.ErrorExchange, Options.ErrorExchange, ErrorKey);
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error setting up queues");
        }

    }

    private readonly ILogger<MessageBusClient> _logger;

    public IModel Channel { get; }
    public MessageBusSettings Options { get; }

    private const string ErrorKey = "*.*.*";

    public void Dispose()
    {
        _logger.LogTrace("Closing and shutting down message bus connection");
        Channel.Dispose();
    }

    /// <summary>
    /// Sends errors to the defined error exchange
    /// </summary>
    /// <param name="data"></param>
    /// <param name="originalKey"></param>
    public void SendError(byte[] data, string originalKey)
    {
        try
        {
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;

            Channel.BasicPublish(Options.ErrorExchange, originalKey, true, properties, data);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "RabbitMQ unable to forward to error exchange {originalKey} {data}", originalKey, data);
        }
    }
}