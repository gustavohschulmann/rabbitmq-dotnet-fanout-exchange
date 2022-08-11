using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FanoutConsumer
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        
        static void Main(string[] args)
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            //Create connection and channel
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            //Create a consumer object
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ConsumerReceived;
            
            //Subscribe to the queue
            var consumerTag = channel.BasicConsume("my.queue1", true, consumer);
            
            Console.WriteLine("Waiting for the messages. Press any key to exit");
            Console.ReadKey();
        }

        private static void ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body.ToArray());
            Console.WriteLine("Message: " + message);
            
            //Acknowledge to rabbitMQ we processed the message with this DeliveryTag and it's ok to remove it from the queue
            channel.BasicAck(e.DeliveryTag, false);
            
            //Or if you want to deny the receive of the message, due to an internal error or something
            //channel.BasicNack(e.DeliveryTag, false, true); //This true tell to put the message again on queue
        }
    }
}