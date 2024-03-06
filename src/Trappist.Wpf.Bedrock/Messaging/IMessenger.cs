using System;
using System.Diagnostics.CodeAnalysis;

namespace Trappist.Wpf.Bedrock.Messaging;

/// <summary>
/// Contracts for the generic messenger.
/// </summary>
public interface IMessenger
{
    /// <summary>
    /// Sends the specified message.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="message">The message.</param>
    void Send<TMessage>(TMessage message);
    /// <summary>
    /// Sends the specified message.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    void Send<TMessage>();
    /// <summary>
    /// Subscribes to the specified message.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="subscriber">The subscriber.</param>
    /// <param name="action">The actions to invoke when a message has been sent.</param>
    void Subscribe<TMessage>([DisallowNull] object subscriber, Action<object?> action);
    /// <summary>
    /// Unsubscribes from the specified message.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="subscriber">The subscriber.</param>
    void Unsubscribe<TMessage>([DisallowNull] object subscriber);
}
