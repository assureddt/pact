using System;

namespace Pact.Core.Extensions;

public static class IntegerExtensions
{
    public static void Times(this int count, Action action)
    {
        for (var i = 0; i < count; i++)
        {
            action();
        }
    }
}