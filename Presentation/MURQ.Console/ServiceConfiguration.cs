using Microsoft.Extensions.DependencyInjection;

using MURQ.Application;
using MURQ.Application.Interfaces;
using MURQ.Application.UrqLoaders;
using MURQ.Infrastructure.ConsoleInterface;
using MURQ.Infrastructure.QuestLoaders;

namespace MURQ.Console;
internal static class ServiceConfiguration
{
    public static ServiceProvider ConfigureServices(string[] args) => new ServiceCollection()
        .AddApplication()
        .AddInfrastructure(qstFilePath: args is [string qstFilePath, ..] ? qstFilePath : null)
        .AddTransient<IVersionProvider, MurqConsoleVersionProvider>()
        .BuildServiceProvider();

    private static IServiceCollection AddInfrastructure(this IServiceCollection services, string? qstFilePath) => services
        .AddTransient<IUserInterface, ConsoleUserInterface>()
        .AddTransient<IQuestLoader, FileQuestLoader>(serviceProvider => new FileQuestLoader(
            urqLoader: serviceProvider.GetRequiredService<UrqLoader>(),
            qstFilePath: qstFilePath));
}
