using Microsoft.Extensions.DependencyInjection;
using SelfHostedGoogleFonts.Storage;
using SelfHostedGoogleFonts.Storage.FileSystem;

// ReSharper disable once CheckNamespace
namespace SelfHostedGoogleFonts;

public static class BuilderExtensions
{
    public static SelfHostedGoogleFontsBuilder AddFileSystemStorage(this SelfHostedGoogleFontsBuilder builder)
    {
        builder.Services.AddTransient<IFontStorage, FileSystemFontStorage>();
        
        return builder;
    }
    
    public static SelfHostedGoogleFontsBuilder AddFileSystemStorage(
        this SelfHostedGoogleFontsBuilder builder,
        Action<FileSystemFontStorageOptions> configureOptions
    )
    {
        builder.Services.Configure(configureOptions);
        return builder.AddFileSystemStorage();
    }
}