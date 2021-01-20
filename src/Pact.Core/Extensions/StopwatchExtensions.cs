using System;
using System.Diagnostics;

namespace Pact.Core.Extensions
{
    public static class StopwatchExtensions
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
    }
}
