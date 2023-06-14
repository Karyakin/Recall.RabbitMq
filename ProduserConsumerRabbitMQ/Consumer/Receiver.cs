using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    public class Receiver
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // create the factory
            using (var connection = factory.CreateConnection()) // open the connection
            using (var chanel = connection.CreateModel()) // open the chanel
            {
                chanel.QueueDeclare( // declare the queue
                    queue: "BasicTest",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                var consumer = new EventingBasicConsumer(chanel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Recieved message: {message}");
                };

                chanel.BasicConsume(
                    queue: "BasicTest",
                    autoAck: true,
                    consumer: consumer
                );
                
                Console.WriteLine("Press key to exit consumer");
                Console.ReadKey();
            }
        }
    }
}