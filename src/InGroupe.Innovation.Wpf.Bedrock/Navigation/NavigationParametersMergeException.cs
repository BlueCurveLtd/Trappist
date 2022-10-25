using System;
using System.Runtime.Serialization;

namespace InGroupe.Innovation.Wpf.Bedrock.Navigation;

[Serializable]
public sealed class NavigationParametersMergeException : Exception
{
    public NavigationParametersMergeException()
    {
    }

    public NavigationParametersMergeException(string? message) : base(message)
    {
    }

    public NavigationParametersMergeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NavigationParametersMergeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
