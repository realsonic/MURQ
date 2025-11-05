using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Console;

using ServiceProvider serviceProvider = ServiceConfiguration.ConfigureServices(args);
using var scope = serviceProvider.CreateScope();
IUrqPlayer urqPlayer = scope.ServiceProvider.GetRequiredService<IUrqPlayer>();

await urqPlayer.Run();