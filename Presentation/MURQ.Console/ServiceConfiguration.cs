using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.Services;
using MURQ.Application.UrqLoaders;
using MURQ.Infrastructure.ConsoleInterface;
using MURQ.Infrastructure.QuestLoaders;

namespace MURQ.Console;
internal class ServiceConfiguration
{
    public static ServiceProvider ConfigureServices(string[] args) => new ServiceCollection()
        .AddTransient<IVersionProvider, MurqConsoleVersionProvider>()
        .AddTransient<IUrqPlayer, UrqPlayer>()
        .AddTransient<IUserInterface, ConsoleUserInterface>()
        .AddTransient<UrqLoader>()
        .AddTransient<IQuestLoader, FileQuestLoader>(serviceProvider => new FileQuestLoader(
            urqLoader: serviceProvider.GetRequiredService<UrqLoader>(),
            qstFilePath: args is [string qstFilePath, ..] ? qstFilePath : null))
        .BuildServiceProvider();
}
