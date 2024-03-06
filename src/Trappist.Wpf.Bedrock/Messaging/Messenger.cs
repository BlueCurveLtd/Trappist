using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Trappist.Wpf.Bedrock.Messaging;

public sealed class Messenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, SynchronizedCollection<Subscription>> subscriptions = new();
    private readonly ConcurrentDictionary<Type, object> currentState = new();

    internal Messenger()
    {

    }

    public void Send<TMessage>()
    {
        this.EnsureSubscriptionDictionaryHasMessageType<TMessage>();

        var messageType = typeof(TMessage);

        _ = this.currentState.AddOrUpdate(messageType, _ => messageType, (_, _) => messageType);

        foreach (var subscription in this.subscriptions[messageType])
        {
            SendMessageToSubscriber(messageType, subscription);
        }
    }

    public void Send<TMessage>(TMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        this.EnsureSubscriptionDictionaryHasMessageType<TMessage>();

        var messageType =  typeof(TMessage);

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

        if (this.currentState.TryGetValue(messageType, out var value))
        {
            SendMessageToSubscriber(value, subscription);
        }
    }

    public void Unsubscribe<TMessage>(object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber, nameof(subscriber));

        var messageType = typeof(TMessage);

        if (!this.subscriptions.TryGetValue(messageType, out var value))
            return;

        var subscription = value.FirstOrDefault(x => x.Subscriber == subscriber);

        if (subscription is not null)
        {
            value.Remove(subscription);
        }
    }

    private void EnsureSubscriptionDictionaryHasMessageType<TMessage>()
        => this.EnsureSubscriptionDictionaryHasMessageType(typeof(TMessage));

    private void EnsureSubscriptionDictionaryHasMessageType(Type typeofMessage)
    {
        if (!this.subscriptions.ContainsKey(typeofMessage))
        {
            this.subscriptions.TryAdd(typeofMessage, []);
        }
    }

    private static void SendMessageToSubscriber<TMessage>(TMessage message, Subscription subscription) => subscription.Action(message);
}
