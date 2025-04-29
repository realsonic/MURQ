using MURQ.Application.Interfaces;

using System.Reflection;

namespace MURQ.Console;

internal class MurqConsoleVersionProvider : IVersionProvider
{
    public string Version => GetVersion();

    private static string GetVersion()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        return assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
            ?? assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? "(ошибка получения версии)";
    }
}