using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers;

[HtmlTargetElement("input")]
[HtmlTargetElement("select")]
public class BootstrapInputTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (output.Attributes.ContainsName("type") && output.Attributes["type"].Value.ToString() != "checkbox")
            output.AddClass("form-control", HtmlEncoder.Default);
    }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if ((output.Attributes.ContainsName("type") && output.Attributes["type"].Value.ToString() != "checkbox")
            || output.TagName == "select")
            output.AddClass("form-control", HtmlEncoder.Default);
        return Task.CompletedTask;
    }
}