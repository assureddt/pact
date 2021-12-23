using System.Diagnostics;

namespace Pact.Core.Helpers;

public static class StopwatchHelper
{
    /// <summary>
    /// Helper to wrap an action in a stopwatch in order to time it
    /// </summary>
    /// <param name="action">The action to be timed</param>
    /// <returns>How long the action took to execute</returns>
    public static TimeSpan Time(Action action)
    {
        var sw = Stopwatch.StartNew();

        action();

        sw.Stop();

        return sw.Elapsed;
    }

    /// <summary>
    /// Helper to wrap an asynchronous function in a stopwatch in order to time it
    /// </summary>
    /// <param name="func">The asynchronous function to be timed</param>
    /// <returns>How long the function took to execute</returns>
    public static async Task<TimeSpan> TimeAsync(Func<Task> func)
    {
        var sw = Stopwatch.StartNew();

        await func();

        sw.Stop();

        return sw.Elapsed;
    }
}