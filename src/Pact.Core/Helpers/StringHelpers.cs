using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Pact.Core.Helpers
{
    public static class StringHelpers
    {
        public static string ToJson<T>(this T obj, bool indent = false, bool ignoreNull = false, bool stringEscape = true) where T : class
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb);
            using var writer = new JsonTextWriter(sw);
            var ser = new JsonSerializer
            {
                Formatting = indent ? Formatting.Indented : Formatting.None,
                NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include,
                StringEscapeHandling = stringEscape ? StringEscapeHandling.EscapeHtml : StringEscapeHandling.Default
            };
            ser.Serialize(writer, obj);

            return sb.ToString();
        }

        public static T FromJson<T>(this string value, bool relaxedArrayEnclosure = false) where T : class
        {
            if (relaxedArrayEnclosure && typeof(T).IsArray)
            {
                if (!value.TrimStart().StartsWith("[")) value = "[" + value;
                if (!value.TrimEnd().EndsWith("]")) value += "]";
            }

            using var sw = new StringReader(value);
            using var reader = new JsonTextReader(sw);
            var ser = new JsonSerializer();
            return ser.Deserialize<T>(reader);
        }
    }
}
