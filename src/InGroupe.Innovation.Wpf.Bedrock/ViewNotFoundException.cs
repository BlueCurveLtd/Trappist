using System;
using System.Runtime.Serialization;

namespace InGroupe.Innovation.Wpf.Bedrock;

[Serializable]
public class ViewNotFoundException : Exception
{
    public ViewNotFoundException()
    {
    }

    public ViewNotFoundException(string? message) : base(message)
    {
    }

    public ViewNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ViewNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
