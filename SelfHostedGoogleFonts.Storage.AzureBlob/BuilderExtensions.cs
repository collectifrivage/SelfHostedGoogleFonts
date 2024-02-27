using Microsoft.Extensions.DependencyInjection;
using SelfHostedGoogleFonts.Storage.AzureBlobStorage;

// ReSharper disable once CheckNamespace
using SelfHostedGoogleFonts.Storage;

namespace SelfHostedGoogleFonts;

public static class BuilderExtensions
{
    public static SelfHostedGoogleFontsBuilder AddAzureBlobStorage(this SelfHostedGoogleFontsBuilder builder)
    {
        builder.Services.ConfigureOptions<AzureBlobFontStorageOptions>();
        builder.Services.AddTransient<IFontStorage, AzureBlobFontStorage>();
        
        return builder;
    }
    
    public static SelfHostedGoogleFontsBuilder AddAzureBlobStorage(
        this SelfHostedGoogleFontsBuilder builder,
        Action<AzureBlobFontStorageOptions> configureOptions
    )
    {
        builder.Services.Configure(configureOptions);
        return builder.AddAzureBlobStorage();
    }
}