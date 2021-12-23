using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pact.Core.Extensions;

namespace Pact.Web.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Prepares the content of the attachment header with safe escaping
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string MakeSafeAttachmentHeader(this string filename)
    {
        // NOTE: was doing a bunch of stuff here to make things work for old browsers, but we're ditching them anyway
        // https://stackoverflow.com/questions/93551/how-to-encode-the-filename-parameter-of-content-disposition-header-in-http
        return $"attachment; filename*=UTF-8''{Uri.EscapeDataString(filename)}";
    }

    /// <summary>
    /// Wraps a string with double-quotes whilst escaping the internal quotes for passing into javascript
    /// </summary>
    /// <param name="html"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static IHtmlContent ForJs(this IHtmlHelper html, string content) => html.Raw(content.ToJson());

    /// <summary>
    /// Wraps a string with double-quotes whilst escaping the internal quotes for passing into javascript
    /// </summary>
    /// <param name="html"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static IHtmlContent ForJs(this IHtmlHelper html, HtmlString content) => html.ForJs(content.Value);
}