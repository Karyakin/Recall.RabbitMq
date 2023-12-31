﻿namespace MicroRabbit.Banking.Domain.Events;

public class TransferCreatedEvent : MicroRabbit.Domain.Core.Events.Event
{
    public TransferCreatedEvent(int from, int to, decimal amount)
    {
        From = from;
        To = to;
        Amount = amount;
    }

    public int From { get; private set; }
    public int To { get; private set; }
    public decimal Amount { get; private set; }
}