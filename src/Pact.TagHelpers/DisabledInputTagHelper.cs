using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers
{
    [HtmlTargetElement("input", Attributes = ForAttributeName)]
    [HtmlTargetElement("select", Attributes = ForAttributeName)]
    public class DisabledInputTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-is-disabled";

        [HtmlAttributeName(ForAttributeName)]
        public bool IsDisabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsDisabled)
                output.Attributes.Add("disabled", "disabled");
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (IsDisabled)
                output.Attributes.Add("disabled", "disabled");
            return Task.CompletedTask;
        }
    }
}