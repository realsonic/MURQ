using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.UrqLoaders;
using MURQ.Infrastructure.ConsoleInterface;

using QuestSources.FileSource;

using System.Text;

namespace MURQ.Infrastructure;

public static class ServiceConfiguration
{
    public static IServiceCollection AddConsoleUserInterface(this IServiceCollection services) => services
        .AddTransient<IUserInterface, ConsoleUserInterface>();

    public static IServiceCollection AddFileQuestSource(this IServiceCollection services, string[] args)
    {
        string? filePath = args is [string qstFilePath, ..] ? qstFilePath : null;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding encoding = Encoding.GetEncoding("Windows-1251");

        return services
            .AddTransient<IQuestSource, FileQuestSource>(serviceProvider => new FileQuestSource(serviceProvider.GetRequiredService<UrqLoader>(), filePath, encoding));
    }
}
