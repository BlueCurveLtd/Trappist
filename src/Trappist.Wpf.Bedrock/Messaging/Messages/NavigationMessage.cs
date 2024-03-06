using System;

namespace Trappist.Wpf.Bedrock.Messaging.Messages;

public sealed record class NavigationMessage(Type ViewType, Uri ViewUri);
