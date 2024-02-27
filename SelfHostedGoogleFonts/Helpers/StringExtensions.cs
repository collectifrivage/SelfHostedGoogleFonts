using System.Security.Cryptography;
using System.Text;

namespace SelfHostedGoogleFonts.Helpers;

internal static class StringExtensions
{
    public static string Sha256(this string value)
    {
        var data = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(data);
        
        var result = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            result.AppendFormat(b.ToString("x2"));
        }

        return result.ToString();
    }
}