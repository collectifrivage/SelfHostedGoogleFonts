using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace SelfHostedGoogleFonts.Storage.AzureBlobStorage;

public class AzureBlobFontStorage(IOptions<AzureBlobFontStorageOptions> options) : IFontStorage
{
    public async Task<bool> FileExistsAsync(string filename)
    {
        var blob = GetBlob(filename);
        var response = await blob.ExistsAsync().ConfigureAwait(false);
        return response.Value;
    }

    public async Task StoreAssetAsync(string filename, Stream data, string? contentType)
    {
        var blob = GetBlob(filename);
        await blob.UploadAsync(
            data,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            });
    }

    public async Task StoreStylesheetAsync(string filename, string content)
    {
        var blob = GetBlob(filename);
        
        await blob.UploadAsync(
            BinaryData.FromString(content),
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "text/css" }
            }
        );
    }

    public string GetFileUrl(string filename)
    {
        var blob = GetBlob(filename);

        var customUrl = options.Value.CustomUrl;
        
        if (string.IsNullOrWhiteSpace(customUrl))
            return blob.Uri.AbsoluteUri;
        else
            return $"{customUrl.TrimEnd('/')}/{blob.Name}";
    }

    private BlobClient GetBlob(string filename)
    {
        return new BlobClient(options.Value.BlobConnectionString, options.Value.BlobContainerName, filename);
    }
}