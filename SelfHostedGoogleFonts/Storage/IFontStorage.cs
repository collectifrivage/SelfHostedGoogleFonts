namespace SelfHostedGoogleFonts.Storage;

public interface IFontStorage
{
    Task<bool> FileExistsAsync(string filename);
    Task StoreFileAsync(string filename, Stream data);
    Task StoreFileAsync(string filename, string content);
    string GetFileUrl(string filename);
}