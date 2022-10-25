using System;
using System.Runtime.Serialization;

namespace Trappist.Wpf.Bedrock;

[Serializable]
public class ViewAliasAlreadyExistsException : Exception
{
    public ViewAliasAlreadyExistsException()
    {
    }

    public ViewAliasAlreadyExistsException(string? message) : base(message)
    {
    }

    public ViewAliasAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ViewAliasAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
