using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Console;

ServiceProvider serviceProvider = ServiceConfiguration.ConfigureServices(args);
IUrqPlayer urqPlayer = serviceProvider.GetRequiredService<IUrqPlayer>();

await urqPlayer.Run();