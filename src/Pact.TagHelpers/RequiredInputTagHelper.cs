using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers;

[HtmlTargetElement("input", Attributes = ForAttributeName)]
[HtmlTargetElement("select", Attributes = ForAttributeName)]
public class RequiredInputTagHelper : TagHelper
{
    private const string ForAttributeName = "asp-required";

    [HtmlAttributeName(ForAttributeName)]
    public bool Required { get; set; }


    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Required)
            output.Attributes.Add("required", "required");
    }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (Required)
            output.Attributes.Add("required", "required");
        return Task.CompletedTask;
    }
}