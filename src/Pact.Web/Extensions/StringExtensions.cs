using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Pact.Core.Extensions;
using Unidecode.NET;

namespace Pact.Web.Extensions
{
    public static class StringExtensions
    {
        public static string MakeSafeAttachmentHeader(this string filename, char replaceChar = '_')
        {
            var safename = filename.MakeSafeFilename(replaceChar);
            var ascii = safename.Unidecode();

            // https://stackoverflow.com/questions/93551/how-to-encode-the-filename-parameter-of-content-disposition-header-in-http
            return $"attachment; filename=\"{ascii}\"; filename*=UTF-8''{Uri.EscapeDataString(safename)}";
        }

        // these just wrap a string with double-quotes whilst escaping the internal quotes for passing into javascript
        public static IHtmlContent ForJs(this IHtmlHelper html, string content) => html.Raw(JsonConvert.SerializeObject(content));
        public static IHtmlContent ForJs(this IHtmlHelper html, IHtmlContent content) => html.ForJs(content.ToString());

        public static string FormatStringArrayJson(this string value)
        {
            var values = value?.Trim('[',']')?.Split(',', StringSplitOptions.RemoveEmptyEntries)?.Select(x => $"\"{x.Trim('\"')}\"")?.ToArray();
            return values?.Any() != true ? "[]" : $"[{string.Join(",", values)}]";
        }

        public static string[] GetLines(this string value, int wrapAt, int maxLength = 250)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new [] { "" };

            value = value.Ellipsis(maxLength);

            var lines = new List<string>();
            var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var thisLine = new StringBuilder();

            foreach (var word in words)
            {
                if (thisLine.Length + word.Length + 1 < wrapAt)
                {
                    thisLine.AppendFormat(" {0}", word);
                }
                else
                {
                    lines.Add(thisLine.ToString());
                    thisLine.Clear();
                    thisLine.AppendFormat("{0}", word);
                }
            }

            lines.Add(thisLine.ToString());

            return lines.ToArray();
        }
    }
}
