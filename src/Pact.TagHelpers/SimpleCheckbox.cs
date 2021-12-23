using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers;

[HtmlTargetElement("checkbox")]
public class SimpleCheckbox : TagHelper
{
    [HtmlAttributeName("asp-is-disabled")]
    public bool IsDisabled { get; set; }

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "input";

        output.Attributes.Add("type", "checkbox");
        if(!string.IsNullOrWhiteSpace(For?.Name))
            output.Attributes.Add("name", For.Name);

        if (IsDisabled)
            output.Attributes.Add("disabled", "disabled");

        if(For?.Model is bool model && model)
            output.Attributes.Add("checked", "checked");
    }
}