using Microsoft.Extensions.DependencyInjection;

using MURQ.Application.Interfaces;
using MURQ.Console;

using ServiceProvider serviceProvider = ServiceConfiguration.ConfigureServices(args);
IUrqPlayer urqPlayer = serviceProvider.GetRequiredService<IUrqPlayer>();

await urqPlayer.Run();