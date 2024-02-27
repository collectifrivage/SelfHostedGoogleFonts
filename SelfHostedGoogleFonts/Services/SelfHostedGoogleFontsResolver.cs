using System.Collections.Concurrent;
using SelfHostedGoogleFonts.Helpers;
using SelfHostedGoogleFonts.Storage;

namespace SelfHostedGoogleFonts.Services;

public class SelfHostedGoogleFontsResolver(IFontStorage storage, IHttpClientFactory httpClientFactory)
{
    private readonly ConcurrentDictionary<string, string> _cache = new();
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    
    public async Task<string> GetSelfHostedStylesheetUrlAsync(params string[] fontSpecs)
    {
        Array.Sort(fontSpecs);

        var cacheKey = string.Join("|", fontSpecs);
        var stylesheetUrl = await _cache.GetOrAddAsync(cacheKey, _ => GetSelfHostedStylesheetUrlInternal(fontSpecs))
            .ConfigureAwait(false);

        return stylesheetUrl;
    }

    private async Task<string> GetSelfHostedStylesheetUrlInternal(string[] sortedFontSpecs)
    {
        var sourceUrl = CreateGoogleFontsUrl(sortedFontSpecs);
        var expectedFilename = $"stylesheets/{sourceUrl.Sha256()}.css";

        if (!await storage.FileExistsAsync(expectedFilename).ConfigureAwait(false))
        {
            var sourceCss = await _httpClient.GetStringAsync(sourceUrl).ConfigureAwait(false);
            var processedCss = await CssHelper.ProcessUrlsAsync(
                    sourceCss,
                    async resourceUrl => await GetSelfHostedResourceUrl(resourceUrl))
                .ConfigureAwait(false);

            await storage.StoreFileAsync(expectedFilename, processedCss);
        }

        return storage.GetFileUrl(expectedFilename);
    }

    private async Task<string> GetSelfHostedResourceUrl(string sourceUrl)
    {
        var extension = sourceUrl.Split('.').Last();
        var filename = $"fonts/{sourceUrl.Sha256()}.{extension}";

        if (!await storage.FileExistsAsync(filename))
        {
            var data = await _httpClient.GetStreamAsync(sourceUrl).ConfigureAwait(false);
            await storage.StoreFileAsync(filename, data);
        } 
                            
        return storage.GetFileUrl(filename);
    }

    private string CreateGoogleFontsUrl(string[] fontSpecs)
    {
        const string baseUrl = "https://fonts.googleapis.com/css2";
        var queryString = string.Join("&", fontSpecs.Select(x => $"family={x}"));
        return $"{baseUrl}?{queryString}";
    }
}