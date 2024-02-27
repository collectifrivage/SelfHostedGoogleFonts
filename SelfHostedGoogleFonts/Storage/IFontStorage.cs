namespace SelfHostedGoogleFonts.Storage;

public interface IFontStorage
{
    Task<bool> FileExistsAsync(string filename);
    Task StoreAssetAsync(string filename, Stream data, string? contentType);
    Task StoreStylesheetAsync(string filename, string content);
    string GetFileUrl(string filename);
}