using System.Collections.Concurrent;

namespace SelfHostedGoogleFonts.Helpers;

internal static class ConcurrentDictionaryExtensions
{
    public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dict, 
        TKey key, 
        Func<TKey, Task<TValue>> valueFactory) 
        where TKey : notnull
    {
        if (dict.TryGetValue(key, out var value))
            return value;
        
        return dict.GetOrAdd(key, await valueFactory(key).ConfigureAwait(false));
    }
}