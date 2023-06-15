using MediatR;

namespace MicroRabbit.Domain.Core.Events;

public  class Message : IRequest<bool>
{
    public string MessageType { get; protected set; }

    protected Message()
    {
        MessageType = GetType().Name;
    }
}