using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers
{
    [HtmlTargetElement("option", Attributes = ForAttributeName)]
    public class OptionTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper
    {
        private const string ForAttributeName = "asp-selected";

        [HtmlAttributeName(ForAttributeName)]
        public bool Selected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Selected)
                output.Attributes.Add("selected", "selected");
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Selected)
                output.Attributes.Add("selected", "selected");
            return Task.CompletedTask;
        }

        public OptionTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }
    }
}