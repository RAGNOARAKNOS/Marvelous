using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace MarvelRabbit
{
    class Send
    {
        public static void TX()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("hello", false, false, false, null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", "hello", null, body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
    class Receive
    {
        public static void RX()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queue: "hello", noAck: true, consumer: consumer);

                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");
                while (true)
                {
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Send.TX();
            Receive.RX();
        }
    }
}
