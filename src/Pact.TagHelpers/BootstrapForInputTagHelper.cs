using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers
{
    [HtmlTargetElement("input", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("select", Attributes = ForAttributeName)]
    public class BootstrapForInputTagHelper : InputTagHelper
    {
        private const string ForAttributeName = "asp-for";

        public BootstrapForInputTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var inputType = string.IsNullOrEmpty(InputTypeName) ? GetInputType(For.ModelExplorer, out _) : InputTypeName.ToLowerInvariant();

            if (inputType != "checkbox")
                output.AddClass("form-control", HtmlEncoder.Default);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var inputType = string.IsNullOrEmpty(InputTypeName) ? GetInputType(For.ModelExplorer, out _) : InputTypeName.ToLowerInvariant();

            if (inputType != "checkbox")
                output.AddClass("form-control", HtmlEncoder.Default);
            return Task.CompletedTask;
        }
    }
}