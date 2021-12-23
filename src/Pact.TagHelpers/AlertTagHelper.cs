using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers;

[HtmlTargetElement("alert")]
public class AlertTagHelper : TagHelper
{
    [HtmlAttributeName("alert-type")]
    public AlertTypes AlertType { get; set; }

    [HtmlAttributeName("alert-message")]
    public string AlertMessage { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagMode = TagMode.StartTagAndEndTag;
        if (!string.IsNullOrWhiteSpace(AlertMessage))
        {
            output.Content.SetHtmlContent(
                $"<div class='alert alert-{AlertType}' role='alert'>{new HtmlString(AlertMessage)}</div>");
        }
        else
        {
            var content = (await output.GetChildContentAsync()).GetContent();
            output.Content.SetHtmlContent(
                $"<div class='alert alert-{AlertType}' role='alert'>{content}</div>");
        }

        await base.ProcessAsync(context, output);
    }
}

public enum AlertTypes
{
    danger,
    warning,
    info,
    success
}