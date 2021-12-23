using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers;

[HtmlTargetElement(Attributes = AttributeName)]
public class IsReadOnlyTagHelper : TagHelper
{
    private const string AttributeName = "is-read-only";

    [HtmlAttributeName(AttributeName)]
    public bool IsReadOnly { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!IsReadOnly) return;

        var useReadOnly = !context.TagName.Equals("select", StringComparison.OrdinalIgnoreCase);

        if (context.TagName.Equals("input", StringComparison.OrdinalIgnoreCase))
        {
            if (context.AllAttributes.ContainsName("type"))
            {
                var type = context.AllAttributes["type"].Value.ToString();
                if (type.Equals("radio", StringComparison.OrdinalIgnoreCase) ||
                    type.Equals("checkbox", StringComparison.OrdinalIgnoreCase))
                    useReadOnly = false;
            }

            if (context.AllAttributes.ContainsName("class"))
            {
                var c = context.AllAttributes["class"].Value.ToString();
                var split = c.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (split.Any(x => x.Equals("checkbox", StringComparison.OrdinalIgnoreCase)))
                    useReadOnly = false;
            }
        }

        if (useReadOnly)
            output.Attributes.Add("readonly", "readonly");
        else
            output.Attributes.Add("disabled", "disabled");
    }
}