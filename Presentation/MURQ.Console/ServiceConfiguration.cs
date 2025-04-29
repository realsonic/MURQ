using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.Services;
using MURQ.Infrastructure.ConsoleInterface;
using MURQ.Infrastructure.QuestLoaders;

namespace MURQ.Console;
internal class ServiceConfiguration
{
    public static ServiceProvider ConfigureServices(string[] args)
    {
        return new ServiceCollection()
            .AddTransient<IVersionProvider, MurqConsoleVersionProvider>()
            .AddTransient<IUrqPlayer, UrqPlayer>()
            .AddTransient<IUserInterface, ConsoleUserInterface>()
            .AddTransient<IQuestLoader, FileQuestLoader>(serviceProvider => new FileQuestLoader(args is [string qstFilePath, ..] ? qstFilePath : null))
            .BuildServiceProvider();
    }
}
