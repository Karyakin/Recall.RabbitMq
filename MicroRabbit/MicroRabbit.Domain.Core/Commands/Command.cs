using MicroRabbit.Domain.Core.Events;

namespace MicroRabbit.Domain.Core.Commands;

public abstract class Command : Message
{
    public DateTime Timestamp { get; protected set; }

    public Command()
    {
        Timestamp = DateTime.Now;
    }
}