using System;

namespace Trappist.Wpf.Bedrock.Messaging;

internal sealed record Subscription(object Subscriber, Action<object?> Action);
