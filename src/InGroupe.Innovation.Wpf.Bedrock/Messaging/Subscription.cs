using System;

namespace InGroupe.Innovation.Wpf.Bedrock.Messaging;

internal sealed record Subscription(object Subscriber, Action<object?> Action);
