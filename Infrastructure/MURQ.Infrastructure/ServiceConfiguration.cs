using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.UrqLoaders;
using MURQ.Common;
using MURQ.Common.Exceptions;
using MURQ.Infrastructure.ConsoleInterface;

using QuestSources.FileSource;

using System.Text;

namespace MURQ.Infrastructure;

public static class ServiceConfiguration
{
    public static IServiceCollection AddConsoleUserInterface(this IServiceCollection services) => services
        .AddSingleton<IUserInterface, ConsoleUserInterface>();

    public static IServiceCollection AddFileQuestSource(this IServiceCollection services, string[] args)
    {
        string? filePath = TryGetFilePathArgument(args);
        Encoding? encoding = TryGetEncodingArgument(args);

        return services
            .AddTransient<IQuestSource, FileQuestSource>(serviceProvider => new FileQuestSource(serviceProvider.GetRequiredService<UrqLoader>(), filePath, encoding));
    }

    private static string? TryGetFilePathArgument(string[] args) => args is [string qstFilePath, ..] ? qstFilePath : null;

    private static Encoding? TryGetEncodingArgument(string[] args)
    {
        if (args is [_, string paramName, string paramValue] && paramName.ToLower() is "--encoding" or "-enc")
        {
            return paramValue.ToLower() switch
            {
                "utf8" => CommonEncodings.UTF8,
                "win" => CommonEncodings.Windows,
                "dos" => CommonEncodings.DOS,
                _ => throw new MurqException($"Неизвестная кодировка файла - {paramValue}")
            };
        }

        return null;
    }
}
