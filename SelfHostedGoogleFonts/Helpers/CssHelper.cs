using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace SelfHostedGoogleFonts.Helpers;

internal static partial class CssHelper
{
    public static async Task<string> ProcessUrlsAsync(string sourceCss, Func<string, Task<string>> processUrl)
    {
        var urls = UrlRegex().Matches(sourceCss).Select(x => x.Groups[1].Value).Distinct().ToList();
        var processedUrls = new ConcurrentDictionary<string, string>();

        var tasks = urls.Select(async url =>
        {
            processedUrls.TryAdd(url, await processUrl(url).ConfigureAwait(false));
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);

        return UrlRegex().Replace(sourceCss, x => $"url('{processedUrls[x.Groups[1].Value]}')");
    }

    [GeneratedRegex("url\\('?(http[^)]+)'?\\)")]
    private static partial Regex UrlRegex();
}