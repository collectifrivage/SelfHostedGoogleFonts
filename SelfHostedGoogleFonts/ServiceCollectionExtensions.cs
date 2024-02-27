using Microsoft.Extensions.DependencyInjection;
using SelfHostedGoogleFonts.Services;

namespace SelfHostedGoogleFonts;

public static class ServiceCollectionExtensions
{
    public static SelfHostedGoogleFontsBuilder AddSelfHostedGoogleFonts(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddHttpClient();
        services.AddSingleton<SelfHostedGoogleFontsResolver>();

        return new SelfHostedGoogleFontsBuilder(services);
    }
}

public class SelfHostedGoogleFontsBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
}