using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using SelfHostedGoogleFonts.Services;

namespace SelfHostedGoogleFonts;

public static class HtmlHelperExtensions
{
    public static async Task<IHtmlContent> AddGoogleFontsAsync(this IHtmlHelper htmlHelper, params string[] fontSpecs)
    {
        var resolver = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<SelfHostedGoogleFontsResolver>();

        var stylesheetUrl = await resolver.GetSelfHostedStylesheetUrlAsync(fontSpecs).ConfigureAwait(false);

        return new HtmlString($"<link href=\"{stylesheetUrl}\" rel=\"stylesheet\">");
    }
}