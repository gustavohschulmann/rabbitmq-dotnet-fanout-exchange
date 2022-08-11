// •	Producer: Application that sends the messages.
// •	Consumer: Application that receives the messages.
// •	Queue: Buffer that stores messages.
// •	Message: Information that is sent from the producer to a consumer through RabbitMQ.
// •	Connection: A TCP connection between your application and the RabbitMQ broker.
// •	Channel: A virtual connection inside a connection. When publishing or consuming messages from a queue - it's all done over a channel.
// •	Exchange: Receives messages from producers and pushes them to queues depending on rules defined by the exchange type. To receive messages, a queue needs to be bound to at least one exchange.

using System.Text;
using RabbitMQ.Client;

namespace FanoutPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection conn;
            IModel channel;

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            //Create connection and channel
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            //Create Exchange and Queues
            channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);
            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);
            
            channel.QueueBind("my.queue1", "ex.fanout", "");
            channel.QueueBind("my.queue2", "ex.fanout", "");
            
            //Send encoded message
            channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Message 1"));
            channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes("Message Number 2"));

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            
            //Delete queues and exchange
            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.fanout");
            
            //Close connection and channel
            channel.Close();
            conn.Close();
        }
    }
}