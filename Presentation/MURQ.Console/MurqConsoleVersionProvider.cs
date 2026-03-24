using MURQ.Application.Interfaces;

using System.Reflection;

namespace MURQ.Console;

internal class MurqConsoleVersionProvider : IVersionProvider
{
    public string VersionPrefix
        => ExecutingAssembly.GetName().Version?.ToString(3)
        ?? ExecutingAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
        ?? "(версия не найдена)";

    public string VersionSuffix
    {
        get
        {
            string informationalVersion = ExecutingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? "(версия не найдена)";

            // Разделяем основную версию и суффикс
            var parts = informationalVersion.Split('-', 2);
            string versionSuffix = parts.Length > 1 ? parts[1] : string.Empty;

            return versionSuffix;
        }
    }

    private static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
}