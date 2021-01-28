using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Pact.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a Json string to its representative object of the defined type
        /// </summary>
        /// <remarks>Internally, this uses either Newtonsoft or System.Text.Json, depending on <see cref="JsonSerialization.Serializer"/>. Defaults to STJ.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="relaxedArrayEnclosure">If true, isn't picky about square brackets wrapping a comma-separated list of objects</param>
        /// <param name="caseInsensitive">Ignore character casing of property names (ignored by Newtonsoft - always insensitive)</param>
        /// <param name="ignoreNull">Ignore null values</param>
        /// <returns></returns>
        public static T FromJson<T>(this string value, bool relaxedArrayEnclosure = false, bool caseInsensitive = true, bool ignoreNull = false)
        {
            // NOTE: newtonsoft.json would return null if this is passed in. System.Text.Json (correctly) throws an exception as an empty string is invalid json
            // ... as such, checking separately
            if (string.IsNullOrWhiteSpace(value)) return default;

            if (relaxedArrayEnclosure && typeof(T).IsArray)
            {
                if (!value.TrimStart().StartsWith("[")) value = "[" + value;
                if (!value.TrimEnd().EndsWith("]")) value += "]";
            }

            if (JsonSerialization.Serializer == JsonImplementation.Newtonsoft)
            {
                return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
                {
                    NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include
                });
            }

            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = caseInsensitive,
                IgnoreNullValues = ignoreNull
            };

            return JsonSerializer.Deserialize<T>(value, opts);
        }

        /// <summary>
        /// Replaces all but the alphanumerics from the string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="replacement">string to use for the replacements</param>
        /// <returns></returns>
        public static string StripNonAlphaNumeric(this string value, string replacement = "_") =>
            new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled).Replace(value, replacement);


        /// <summary>
        /// Replaces invalid filename characters in a string
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="replaceChar"></param>
        /// <returns></returns>
        public static string MakeSafeFilename(this string filename, char replaceChar = '_') =>
            Path.GetInvalidFileNameChars().Aggregate(filename, (current, c) => current.Replace(c, replaceChar).Trim());

        /// <summary>
        /// Extracts clean email addresses from a csv-style string that may contain descriptional info
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetEmailAddresses(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) yield break;
            var matches = Regex.Matches(input, @"[^\s,;<>]+@[^\s,;<>]+");

            foreach (Match match in matches) yield return match.Value.ToLowerInvariant();
        }

        /// <summary>
        /// Simple check for pure A-Z
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsBasicUppercaseLetter(this char c) {
            return c >= 'A' && c <= 'Z';
        }

        /// <summary>
        /// Converts a string to a nullable type.  If the conversion wasn't possible, returns null
        /// </summary>
        /// <typeparam name="T">The desired type</typeparam>
        /// <param name="s">The source string</param>
        /// <returns></returns>
        public static T? ToNullable<T>(this string s) where T : struct
        {
            var result = new T?();
            try
            {
                if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    var conv = TypeDescriptor.GetConverter(typeof(T));
                    result = (T)conv.ConvertFrom(s);
                }
            }
            catch
            {
                // ignored
            }
            return result;
        }

        /// <summary>
        /// Masks a character range in a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static string Masked(this string source, int count, bool invert = false)
        {
            return source.Masked('*', source.Length - count, count, invert);
        }

        /// <summary>
        /// Masks a character range in a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static string Masked(this string source, int start, int count, bool invert = false)
        {
            return source.Masked('*', start, count, invert);
        }

        /// <summary>
        /// Masks a character range in a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maskValue"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="invert"></param>
        /// <returns></returns>
        public static string Masked(this string source, char maskValue, int start, int count, bool invert = false)
        {
            var firstPart = invert ? new string(maskValue, start) : source.Substring(0, start);
            var lastPart = invert ? new string(maskValue, start + count - source.Length) : source.Substring(start + count);
            var middlePart = invert ? source.Substring(start, count) : new string(maskValue, count);

            return firstPart + middlePart + lastPart;
        }

        /// <summary>
        /// Truncates a long string to a specified length and includes an ellipsis string at the end
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static string Ellipsis(this string source, int maximum = 30)
        {
            if (source == null || source.Length <= maximum)
                return source;

            const string ellipsis = "...";

            return $"{source.Substring(0, maximum - ellipsis.Length)}{ellipsis}";
        }

        /// <summary>
        /// Simplifies a big string into a series of lines of maximum specified length, with ellipsis applied at the end of the overall max length (breaks and reassembles on whitespace characters)
        /// </summary>
        /// <param name="value">The full string</param>
        /// <param name="wrapAt">Maximum length of each line</param>
        /// <param name="maxLength">Capped overall length of the string (excess results in ellipsis)</param>
        /// <returns></returns>
        public static string[] GetLines(this string value, int wrapAt, int maxLength = 250)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new[] { "" };

            value = value.Ellipsis(maxLength);

            var lines = new List<string>();
            var words = value.Split(new[]{' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var thisLine = new StringBuilder();

            var padStart = false;

            foreach (var word in words)
            {
                if (thisLine.Length + word.Length + 1 >= wrapAt)
                {
                    lines.Add(thisLine.ToString());
                    thisLine.Clear();
                    padStart = false;
                }

                if (padStart) thisLine.Append(" ");
                thisLine.AppendFormat("{0}", word);
                padStart = true;
            }

            lines.Add(thisLine.ToString());

            return lines.ToArray();
        }
    }
}
