using Microsoft.AspNetCore.Razor.TagHelpers;
using SelfHostedGoogleFonts.Services;

namespace SelfHostedGoogleFonts.TagHelpers;

public class GoogleFontsTagHelper(SelfHostedGoogleFontsResolver resolver) : TagHelper
{
    public string? Spec { get; set; }
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (!string.IsNullOrWhiteSpace(Spec))
        {
            var stylesheetUrl = await resolver.GetSelfHostedStylesheetUrlAsync(Spec).ConfigureAwait(false);

            output.TagName = "link";
            output.TagMode = TagMode.StartTagOnly;
            
            output.Attributes.SetAttribute("href", stylesheetUrl);
            output.Attributes.SetAttribute("rel", "stylesheet");
            
            output.Attributes.RemoveAll("spec");
        }
    }
}