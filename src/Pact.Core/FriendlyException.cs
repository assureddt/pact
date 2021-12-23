using System;

namespace Pact.Core;

public class FriendlyException : Exception
{
    public FriendlyException(string message, Exception inner) : base(message, inner)
    {
    }

    public FriendlyException(string message) : base(message)
    {
    }

    public FriendlyException()
    {
    }
}