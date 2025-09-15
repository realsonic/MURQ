using Microsoft.Extensions.DependencyInjection;

using MURQ.Application;
using MURQ.Application.Interfaces;
using MURQ.Infrastructure;

namespace MURQ.Console;
internal static class ServiceConfiguration
{
    public static ServiceProvider ConfigureServices(string[] args) => new ServiceCollection()
        .AddApplication()
        .AddInfrastructure(args)
        .AddVersionProvider()
        .BuildServiceProvider();

    private static IServiceCollection AddInfrastructure(this IServiceCollection services, string[] args) => services
        .AddConsoleUserInterface()
        .AddFileQuestSource(args);

    private static IServiceCollection AddVersionProvider(this IServiceCollection services) => services
        .AddTransient<IVersionProvider, MurqConsoleVersionProvider>();
}
