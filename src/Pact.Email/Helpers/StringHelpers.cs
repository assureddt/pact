using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pact.Email.Helpers
{
    public static class StringHelpers
    {
        public static IEnumerable<string> GetEmailAddresses(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) yield break;
            var matches = Regex.Matches(input, @"[^\s,;<>]+@[^\s,;<>]+");

            foreach (Match match in matches) yield return match.Value.ToLowerInvariant();
        }
    }
}
