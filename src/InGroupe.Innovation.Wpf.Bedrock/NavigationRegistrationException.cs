using System;
using System.Runtime.Serialization;

namespace InGroupe.Innovation.Wpf.Bedrock;

public sealed class NavigationRegistrationException : Exception
{
    public NavigationRegistrationException()
    {
    }

    public NavigationRegistrationException(string message)
        : base(message)
    {

    }

    public NavigationRegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NavigationRegistrationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
