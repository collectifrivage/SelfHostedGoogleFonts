using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SelfHostedGoogleFonts.Helpers;
using SelfHostedGoogleFonts.Storage;

namespace SelfHostedGoogleFonts.Services;

public class SelfHostedGoogleFontsResolver(
    IFontStorage storage,
    IHttpClientFactory httpClientFactory,
    ILogger<SelfHostedGoogleFontsResolver> logger
)
{
    private readonly ConcurrentDictionary<string, string> _cache = new();
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public Task<string> GetSelfHostedStylesheetUrlAsync(IEnumerable<string> fontSpecs) =>
        GetSelfHostedStylesheetUrlAsync(fontSpecs.ToArray());
    
    public async Task<string> GetSelfHostedStylesheetUrlAsync(params string[] fontSpecs)
    {
        Array.Sort(fontSpecs);

        var cacheKey = string.Join("|", fontSpecs);
        
        logger.LogTrace("{Count} specs requested, cache key: {CacheKey}", fontSpecs.Length, cacheKey);

        var fromCache = true;
        var stylesheetUrl = await _cache.GetOrAddAsync(cacheKey, _ =>
            {
                logger.LogTrace("Cache miss: {CacheKey}", cacheKey);
                fromCache = false;
                return GetSelfHostedStylesheetUrlInternal(fontSpecs);
            })
            .ConfigureAwait(false);

        if (fromCache)
        {
            logger.LogTrace("Stylesheet for {CacheKey} was found in cache: {Url}", cacheKey, stylesheetUrl);
        }

        return stylesheetUrl;
    }

    private async Task<string> GetSelfHostedStylesheetUrlInternal(string[] sortedFontSpecs)
    {
        var sourceUrl = CreateGoogleFontsUrl(sortedFontSpecs);
        var expectedFilename = $"stylesheets/{sourceUrl.Sha256()}.css";

        logger.LogTrace("Source stylesheet URL: {Source} | Expected filename: {Filename}", sourceUrl, expectedFilename);

        if (!await storage.FileExistsAsync(expectedFilename).ConfigureAwait(false))
        {
            logger.LogDebug("{Filename} does not exist in storage, processing", expectedFilename);
            
            logger.LogTrace("Downloading source stylesheet from: {Source}", sourceUrl);
            var sourceCss = await _httpClient.GetStringAsync(sourceUrl).ConfigureAwait(false);
            
            logger.LogTrace("Processing URLs from source file");
            var processedCss = await CssHelper.ProcessUrlsAsync(
                    sourceCss,
                    async resourceUrl => await GetSelfHostedResourceUrl(resourceUrl))
                .ConfigureAwait(false);

            logger.LogTrace("Storing processed stylesheet as {Filename}", expectedFilename);
            await storage.StoreStylesheetAsync(expectedFilename, processedCss).ConfigureAwait(false);

            logger.LogInformation(
                "New stylesheet processed from {Source} and saved to {Filename}", 
                sourceUrl, expectedFilename);
        }
        else
        {
            logger.LogDebug("{Filename} found in storage", expectedFilename);
        }

        return storage.GetFileUrl(expectedFilename);
    }

    private async Task<string> GetSelfHostedResourceUrl(string sourceUrl)
    {
        var extension = sourceUrl.Split('.').Last();
        var filename = $"fonts/{sourceUrl.Sha256()}.{extension}";

        logger.LogTrace("Source asset URL: {Source} | Expected filename: {Filename}", sourceUrl, filename);

        if (!await storage.FileExistsAsync(filename))
        {
            logger.LogDebug("{Filename} does not exist in storage, downloading", filename);
            
            var response = await _httpClient.GetAsync(sourceUrl).ConfigureAwait(false);
            var data = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var contentType = response.Content.Headers.ContentType?.MediaType;
            
            logger.LogTrace("Detected content type: {ContentType}", contentType);
            
            logger.LogTrace("Storing asset as {Filename}", filename);
            await storage.StoreAssetAsync(filename, data, contentType).ConfigureAwait(false);
        }
        else
        {
            logger.LogDebug("{Filename} found in storage", filename);
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