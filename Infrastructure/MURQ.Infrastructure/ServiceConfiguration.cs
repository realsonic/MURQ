using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Application.UrqLoaders;
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return paramValue.ToLower() switch
            {
                "utf8" => Encoding.UTF8,
                "win" => Encoding.GetEncoding("Windows-1251"),
                "dos" => Encoding.GetEncoding("cp866"),
                _ => throw new MurqException($"Неизвестная кодировка файла - {paramValue}")
            };
        }
        return null;
    }
}
