using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace SelfHostedGoogleFonts.Storage.FileSystem;

public class FileSystemFontStorage(
    IOptions<FileSystemFontStorageOptions> options,
    IWebHostEnvironment webHostEnvironment
)
    : IFontStorage
{
    public Task<bool> FileExistsAsync(string filename)
    {
        var path = GetFilePath(filename);
        var exists = File.Exists(path);
        return Task.FromResult(exists);
    }

    public async Task StoreAssetAsync(string filename, Stream data, string? contentType)
    {
        var path = GetFilePath(filename);
        var dir = Path.GetDirectoryName(path) 
                  ?? throw new InvalidOperationException();
        
        Directory.CreateDirectory(dir);
        
        await using var file = File.Create(path);
        await data.CopyToAsync(file).ConfigureAwait(false);
    }

    public async Task StoreStylesheetAsync(string filename, string content)
    {
        var path = GetFilePath(filename);
        var dir = Path.GetDirectoryName(path) 
                  ?? throw new InvalidOperationException();
        
        Directory.CreateDirectory(dir);
        
        await File.WriteAllTextAsync(path, content);
    }

    public string GetFileUrl(string filename)
    {
        return $"/{options.Value.PublicPathPrefix}{filename}";
    }

    private string GetFilePath(string filename)
    {
        var root = options.Value.RootPath;
        if (string.IsNullOrWhiteSpace(root))
            root = Path.Combine(webHostEnvironment.WebRootPath, options.Value.PublicPathPrefix);

        return Path.Combine(root, filename);
    }
}