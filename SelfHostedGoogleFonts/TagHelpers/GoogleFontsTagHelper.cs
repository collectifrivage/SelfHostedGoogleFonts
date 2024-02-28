using Microsoft.AspNetCore.Razor.TagHelpers;
using SelfHostedGoogleFonts.Services;

namespace SelfHostedGoogleFonts.TagHelpers;

[RestrictChildren("google-font")]
public class GoogleFontsTagHelper(SelfHostedGoogleFontsResolver resolver) : TagHelper
{
    public string? Spec { get; set; }
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var fontsContext = new Context();
        if (!string.IsNullOrWhiteSpace(Spec))
            fontsContext.Specs.Add(Spec);

        context.Items[nameof(GoogleFontsTagHelper)] = fontsContext;

        _ = await output.GetChildContentAsync();
        
        if (fontsContext.Specs.Any())
        {
            var stylesheetUrl = await resolver.GetSelfHostedStylesheetUrlAsync(fontsContext.Specs)
                .ConfigureAwait(false);

            output.TagName = "link";
            output.TagMode = TagMode.StartTagOnly;
            
            output.Attributes.SetAttribute("href", stylesheetUrl);
            output.Attributes.SetAttribute("rel", "stylesheet");
            
            output.Attributes.RemoveAll("spec");
        }
        else
        {
            output.SuppressOutput();
        }
    }

    public class Context
    {
        public List<string> Specs { get; } = new();
    } 
}

[HtmlTargetElement("google-font", ParentTag = "google-fonts", TagStructure = TagStructure.WithoutEndTag)]
public class GoogleFontTagHelper : TagHelper
{
    public string? Spec { get; set; }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.SuppressOutput();
        
        if (!context.Items.TryGetValue(nameof(GoogleFontsTagHelper), out var contextObj))
            return Task.CompletedTask;
        if (contextObj is not GoogleFontsTagHelper.Context fontsContext)
            return Task.CompletedTask;

        if (!string.IsNullOrWhiteSpace(Spec))
            fontsContext.Specs.Add(Spec);

        return Task.CompletedTask;
    }
}