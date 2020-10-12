using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Pact.TagHelpers
{
    [HtmlTargetElement("label")]
    public class BootstrapLabelTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.AddClass("control-label", HtmlEncoder.Default);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.AddClass("control-label", HtmlEncoder.Default);
            return Task.CompletedTask;
        }
    }


}