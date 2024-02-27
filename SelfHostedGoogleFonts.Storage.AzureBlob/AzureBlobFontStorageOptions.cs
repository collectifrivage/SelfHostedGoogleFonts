using Microsoft.Extensions.Options;

namespace SelfHostedGoogleFonts.Storage.AzureBlobStorage;

public class AzureBlobFontStorageOptions : IValidateOptions<AzureBlobFontStorageOptions>
{
    /// <summary>
    /// Connection string for an Azure Storage Account.
    /// </summary>
    public string? BlobConnectionString { get; set; }
    
    /// <summary>
    /// Name of the Azure Storage Blob container where the fonts will be saved.
    /// The container WILL NOT be automatically created, it must already exist.
    /// This container should allow public access to the blobs, unless a CustomUrl is configured. 
    /// </summary>
    public string? BlobContainerName { get; set; }
    
    /// <summary>
    /// If set, will override the URL used to load the blobs. This can be used to integrate with a CDN.
    /// <example>
    /// For example: <c>https://cdn.example.com/some-path</c> will create URLs that look like
    /// <c>https://cdn.example.com/some-path/stylesheets/your-font.css</c> 
    /// </example>
    /// </summary>
    public string? CustomUrl { get; set; }
    
    public ValidateOptionsResult Validate(string? name, AzureBlobFontStorageOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BlobConnectionString))
            return ValidateOptionsResult.Fail($"{nameof(options.BlobConnectionString)} is required");
        if (string.IsNullOrWhiteSpace(options.BlobContainerName))
            return ValidateOptionsResult.Fail($"{nameof(options.BlobContainerName)} is required");

        return ValidateOptionsResult.Success;
    }
}