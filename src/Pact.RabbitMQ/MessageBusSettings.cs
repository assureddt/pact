using System.Collections.Generic;

namespace Pact.RabbitMQ
{
    public class MessageBusSettings
    {
        public string Hostname { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<MessageBusExchange> Exchanges { get; set; }
        public string ErrorExchange { get; set; }
    }

    public class MessageBusExchange
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
    }
}
