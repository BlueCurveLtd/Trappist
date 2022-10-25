using System;
using System.Runtime.Serialization;

namespace Trappist.Wpf.Bedrock;

[Serializable]
public sealed class BedrockApplicationException : Exception
{
    public BedrockApplicationException()
    {
    }

    public BedrockApplicationException(string? message) : base(message)
    {
    }

    public BedrockApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public BedrockApplicationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
