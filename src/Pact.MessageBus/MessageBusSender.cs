using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pact.Core.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pact.MessageBus
{
    public class MessageBusSender : IMessageBusSender
    {
        public MessageBusSender (ILogger<MessageBusSender> logger, IMessageBusClient client)
        {
            _logger = logger;
            _client = client;

            _rpcQueue = _client.Channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(_client.Channel);
            consumer.Received += Consumer_Received;
            _client.Channel.BasicConsume(consumer: consumer, queue: _rpcQueue, autoAck: true);

            _pendingMessages = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var correlationId = e.BasicProperties.CorrelationId;
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
 
                _logger.LogTrace("Received: {message} with CorrelationId {correlationId}", message, correlationId);
 
                _pendingMessages.TryRemove(correlationId, out var tcs);
                tcs?.SetResult(message);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error processing RPC response");
                throw;
            }
        }

        private readonly ILogger<MessageBusSender> _logger;
        private readonly IMessageBusClient _client;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingMessages;
        private readonly string _rpcQueue;

        public void Send(object item, string exchange, string key)
        {
            try
            {
                var json = item.ToJson();
                var data = Encoding.UTF8.GetBytes(json);

                var properties = _client.Channel.CreateBasicProperties();
                properties.Persistent = true;

                _client.Channel.BasicPublish(exchange, key,  true, properties, data);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error Send {key} {exchange} {item}", exchange, key, item);
                throw;
            }
        }

        public async Task<T> SendRPCAsync<T>(object item, string exchange, string key) where T : class
        {
            try
            {
                var tcs = new TaskCompletionSource<string>();
                var correlationId = Guid.NewGuid().ToString();
            
                _pendingMessages[correlationId] = tcs;

                //Ok call remote server
                var props = _client.Channel.CreateBasicProperties();
                props.CorrelationId = correlationId;
                props.ReplyTo = _rpcQueue;

                var messageJson = item.ToJson();
                var messageData = Encoding.UTF8.GetBytes(messageJson);

                _client.Channel.BasicPublish(exchange, key, props, messageData);
                _logger.LogTrace("Sent: {messageData} with CorrelationId {correlationId}", messageData, correlationId);

                var rawJson = await tcs.Task;
                return rawJson.FromJson<T>();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error Send RPC {key} {exchange} {item}", exchange, key, item);
                throw;
            }
        }
    }
}
