using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.UrqLoaders;
using MURQ.Infrastructure.ConsoleInterface;

using QuestSources.FileSource;

namespace MURQ.Infrastructure;

public static class ServiceConfiguration
{
    public static IServiceCollection AddConsoleUserInterface(this IServiceCollection services) => services
        .AddTransient<IUserInterface, ConsoleUserInterface>();

    public static IServiceCollection AddFileQuestSource(this IServiceCollection services, string[] args)
    {
        string? filePath = args is [string qstFilePath, ..] ? qstFilePath : null;

        return services
            .AddTransient<IQuestSource, FileQuestSource>(serviceProvider => new FileQuestSource(filePath, serviceProvider.GetRequiredService<UrqLoader>()));
    }
}
