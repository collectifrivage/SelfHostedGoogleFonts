# Azure Blob storage for SelfHostedGoogleFonts

With this package you can mirror Google Fonts to an Azure Storage Account.

Refer to the base `SelfHostedGoogleFonts` package's documentation for basic usage.

## How to use

Create an Azure Storage Account, and a blob container. The container must be configured to allow public blob access.

You should also configure a CORS rule on the storage account to allow blob access from your domain name(s), or all domains. This can be done from the Azure portal, or other methods.

Once your storage account is ready, add this to your `Program.cs`:

```csharp
builder.Services.AddSelfHostedGoogleFonts()
    .AddAzureBlobStorage(options =>
    {
        options.BlobConnectionString = "<your-connection-string>";
        options.BlobContainerName = "google-fonts";
    });
```

You can map the options to your app's configuration like this:
```csharp
builder.Services.AddSelfHostedGoogleFonts()
    .AddAzureBlobStorage();

builder.Services.Configure<AzureBlobFontStorageOptions>("fonts:azureStorage");
```

And in your appsettings.json:
```json
{
  "fonts": {
    "azureStorage": {
      "blobConnectionString": "<your-connection-string>",
      "blobContainerName": "google-fonts"
    }
  }
}
```

## Using a custom URL

If you don't want your site to load the fonts directly from the storage account URL (for example, if you want to use a CDN), you can set the `AzureBlobFontStorageOptions.CustomUrl` option.

For example, setting `https://cdn.example.com/some-path` will create URLs that look like
`https://cdn.example.com/some-path/stylesheets/your-font.css`. You are responsible for ensuring these URLs read the files stored in the storage account.