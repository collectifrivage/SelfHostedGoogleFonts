namespace SelfHostedGoogleFonts.Storage.FileSystem;

public class FileSystemFontStorageOptions
{
    /// <summary>
    /// The physical folder where the fonts and stylesheets will be saved.
    /// This defaults to <c>$"{IWebHostEnvironment.WebRootPath}/{PublicPathPrefix}"</c>.
    /// </summary>
    public string? RootPath { get; set; }

    /// <summary>
    /// A URL segment added in front of all self-hosted resource URLs. 
    /// </summary>
    public string PublicPathPrefix { get; set; } = "google-fonts/";
}