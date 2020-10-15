using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Pact.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToJson<T>(this T obj, bool indent = false, bool ignoreNull = false, bool stringEscape = true) where T : class
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = indent ? Formatting.Indented : Formatting.None,
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include,
                StringEscapeHandling = stringEscape ? StringEscapeHandling.EscapeHtml : StringEscapeHandling.Default
            });
        }

        public static T FromJson<T>(this string value, bool relaxedArrayEnclosure = false) where T : class
        {
            if (!relaxedArrayEnclosure || !typeof(T).IsArray) return JsonConvert.DeserializeObject<T>(value);

            if (!value.TrimStart().StartsWith("[")) value = "[" + value;
            if (!value.TrimEnd().EndsWith("]")) value += "]";

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string ToAlphaNumeric(this string value) => new Regex("[^a-zA-Z0-9]").Replace(value, "_");

        public static string DefaultIfNull(this string value, string defaultValue) => !string.IsNullOrWhiteSpace(value) ? value : defaultValue;

        public static string MakeSafeFilename(this string filename, char replaceChar = '_') => Path.GetInvalidFileNameChars().Aggregate(filename, (current, c) => current.Replace(c, replaceChar).Trim());

        public static IEnumerable<string> GetEmailAddresses(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) yield break;
            var matches = Regex.Matches(input, @"[^\s,;<>]+@[^\s,;<>]+");

            foreach (Match match in matches) yield return match.Value.ToLowerInvariant();
        }

        public static bool IsBasicUppercaseLetter(this char c) {
            return c >= 'A' && c <= 'Z';
        }

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

        public static string Masked(this string source, int count, bool invert = false)
        {
            return source.Masked('*', source.Length - count, count, invert);
        }

        public static string Masked(this string source, int start, int count, bool invert = false)
        {
            return source.Masked('*', start, count, invert);
        }

        public static string Masked(this string source, char maskValue, int start, int count, bool invert = false)
        {
            var firstPart = invert ? new string(maskValue, start) : source.Substring(0, start);
            var lastPart = invert ? new string(maskValue, start + count - source.Length) : source.Substring(start + count);
            var middlePart = invert ? source.Substring(start, count) : new string(maskValue, count);

            return firstPart + middlePart + lastPart;
        }

        public static string Ellipsis(this string source, int maximum = 30)
        {
            if (source == null || source.Length <= maximum)
                return source;

            const string ellipsis = "...";

            return $"{source.Substring(0, maximum - ellipsis.Length)}{ellipsis}";
        }
    }
}
