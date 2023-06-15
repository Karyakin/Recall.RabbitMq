﻿using System.Text;
using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroRabbit.Infra.Bus;

public sealed class RabbitMQBus : IEventBus
{
    private readonly IMediator _mediator;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;

    public RabbitMQBus(IMediator mediator)
    {
        _mediator = mediator;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return _mediator.Send(command);
    }

    public void Publish<T>(T @event) where T : Event
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var chanel = connection.CreateModel())
        {
            var eventName = @event.GetType().Name;
            chanel.QueueDeclare(
                queue: eventName,
                durable: false,
                exclusive: false,
                arguments: null,
                autoDelete: true
            );

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            chanel.BasicPublish(
                exchange: string.Empty,
                routingKey: eventName,
                basicProperties: null,
                body: body
            );
        }
    }


    public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(s => s.GetType() == handlerType))
        {
            throw new ArgumentException($"Handler type {handlerType.Name} already is registered for '{eventName}'",
                nameof(handlerType));
        }

        _handlers[eventName].Add(handlerType);

        StartBasicConsume<T>();
    }

    private void StartBasicConsume<T>() where T : Event
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            DispatchConsumersAsync = true
        };

        var connection = factory.CreateConnection();
        var chanel = connection.CreateModel();

        var eventName = typeof(T).Name;

        chanel.QueueDeclare(
            queue: eventName,
            durable: false,
            exclusive: false,
            autoDelete: true,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(chanel);
        consumer.Received += Consumer_Received;

        chanel.BasicConsume(
            queue: eventName,
            autoAck: false,
            consumer: consumer
        );
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        var message = Encoding.UTF8.GetString(e.Body.ToArray());

        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            
            
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (_handlers.ContainsKey(eventName))
        {
            var subscriptions = _handlers[eventName];
            foreach (var subscription in subscriptions)
            {
                var handler = Activator.CreateInstance(subscription);
                if (handler == null)
                    continue;

                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                
                var @event = JsonConvert.DeserializeObject(message, eventType);
                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
            }
        }
    }
}