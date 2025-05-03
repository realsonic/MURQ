using MURQ.Application.Interfaces;

using System.Reflection;

namespace MURQ.Console;

internal class MurqConsoleVersionProvider : IVersionProvider
{
    public string Version => GetVersion();

    private static string GetVersion()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        return assembly.GetName().Version?.ToString(3)
            ?? assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
            ?? assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? "(версия не найдена)";
    }
}