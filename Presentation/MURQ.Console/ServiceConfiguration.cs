using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.Services;
using MURQ.Application.UrqLoaders;
using MURQ.Application.UrqLoaders.UrqStrings;
using MURQ.Infrastructure.ConsoleInterface;
using MURQ.Infrastructure.QuestLoaders;

namespace MURQ.Console;
internal static class ServiceConfiguration
{
    public static ServiceProvider ConfigureServices(string[] args) => new ServiceCollection()
        .AddTransient<IVersionProvider, MurqConsoleVersionProvider>()
        .AddTransient<IUrqPlayer, UrqPlayer>()
        .AddTransient<IUserInterface, ConsoleUserInterface>()
        .AddUrqLoaders()
        .AddTransient<IQuestLoader, FileQuestLoader>(serviceProvider => new FileQuestLoader(
            urqLoader: serviceProvider.GetRequiredService<UrqLoader>(),
            qstFilePath: args is [string qstFilePath, ..] ? qstFilePath : null))
        .BuildServiceProvider();

    private static IServiceCollection AddUrqLoaders(this IServiceCollection services) => services
        .AddTransient<UrqLoader>()
        .AddTransient<UrqStringLoader>()
        .AddTransient<UrqStringLexer>();
}
