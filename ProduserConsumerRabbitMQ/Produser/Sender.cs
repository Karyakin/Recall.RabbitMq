using System;
using System.Text;
using RabbitMQ.Client;

namespace Produser
{
    public class Sender
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory(){HostName = "localhost"};// create the factory
            using (var connection = factory.CreateConnection())// open the connection
            using (var chanel = connection.CreateModel())// open the chanel
            {
                chanel.QueueDeclare( // declare the queue
                    queue: "BasicTest",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                
                var message = "Getting started with .net core RabbitMq";// create the message
                var body = Encoding.UTF8.GetBytes(message);// convert message to byte array
                
                chanel.BasicPublish( // publish the message
                    exchange: string.Empty, 
                    routingKey:"BasicTest",
                    basicProperties: null,
                    body: body
               );
                
                Console.WriteLine($"Sent message: {message}");
            }
            Console.WriteLine($"Press key to exit.");
            Console.ReadLine();
        }
    }
}
