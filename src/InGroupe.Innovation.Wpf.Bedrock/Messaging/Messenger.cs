using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace InGroupe.Innovation.Wpf.Bedrock.Messaging;

public sealed class Messenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, SynchronizedCollection<Subscription>> subscriptions = new();
    private readonly ConcurrentDictionary<Type, object> currentState = new();

    internal Messenger()
    {

    }

    public void Send<TMessage>(TMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);             

        this.EnsureSubscriptionDictionaryHasMessageType<TMessage>();

        var messageType = typeof(TMessage);

        _ = this.currentState.AddOrUpdate(messageType, _ => message, (_, _) => message);

        foreach (var subscription in this.subscriptions[messageType])
        {
            SendMessageToSubscriber(message, subscription);
        }
    }

    public void Subscribe<TMessage>(object subscriber, Action<object?> action)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(action);

        this.EnsureSubscriptionDictionaryHasMessageType<TMessage>();

        var subscription = new Subscription(subscriber, action);

        var messageType = typeof(TMessage);

        this.subscriptions[messageType].Add(subscription);

        if (this.currentState.ContainsKey(messageType))
        {
            SendMessageToSubscriber(this.currentState[messageType], subscription);
        }
    }

    public void Unsubscribe<TMessage>(object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber, nameof(subscriber));

        var messageType = typeof(TMessage);

        if (!this.subscriptions.ContainsKey(messageType))
            return;

        var subscription = this.subscriptions[messageType].FirstOrDefault(x => x.Subscriber == subscriber);

        if (subscription is not null)
        {
            this.subscriptions[messageType].Remove(subscription);
        }
    }

    private void EnsureSubscriptionDictionaryHasMessageType<TMessage>()
    {
        if (!this.subscriptions.ContainsKey(typeof(TMessage)))
        {
            this.subscriptions.TryAdd(typeof(TMessage), new SynchronizedCollection<Subscription>());
        }
    }
        
    private static void SendMessageToSubscriber<TMessage>(TMessage message, Subscription subscription) => subscription.Action(message);
}
