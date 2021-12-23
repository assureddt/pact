using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers;

public enum TooltipPlacement { Top, Bottom, Left, Right }

[HtmlTargetElement("input", Attributes = ForAttributeName)]
[HtmlTargetElement("select", Attributes = ForAttributeName)]
public class TooltipInputTagHelper : TagHelper
{
    public TooltipInputTagHelper()
    {
        TooltipPlacement = TooltipPlacement.Right;
    }

    private const string ForAttributeName = "asp-tooltip";

    [HtmlAttributeName(ForAttributeName)]
    public string ToolTip { get; set; }

    [HtmlAttributeName("asp-tooltip-placement")]
    public TooltipPlacement TooltipPlacement { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(ToolTip)) return;

        output.Attributes.Add("title", ToolTip);
        output.Attributes.Add("data-toggle", "tooltip");
        output.Attributes.Add("data-placement", TooltipPlacement.ToString().ToLower());
    }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(ToolTip)) return Task.CompletedTask;

        output.Attributes.Add("title", ToolTip);
        output.Attributes.Add("data-toggle", "tooltip");
        output.Attributes.Add("data-placement", TooltipPlacement.ToString().ToLower());

        return Task.CompletedTask;
    }
}