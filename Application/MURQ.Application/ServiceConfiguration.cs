using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.Services;
using MURQ.Application.UrqLoaders;
using MURQ.Application.UrqLoaders.UrqStrings;

namespace MURQ.Application;
public static class ServiceConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services) => services
        .AddTransient<IUrqPlayer, UrqPlayer>()
        .AddUrqLoaders();

    private static IServiceCollection AddUrqLoaders(this IServiceCollection services) => services
        .AddTransient<UrqLoader>()
        .AddTransient<UrqStringLoader>()
        .AddTransient<UrqStringLexer>();
}
