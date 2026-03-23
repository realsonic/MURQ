using MURQ.Application.Interfaces;

using System.Reflection;

namespace MURQ.Console;

internal class MurqConsoleVersionProvider : IVersionProvider
{
    public string Version
    {
        get
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version?.ToString(3)
                ?? assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
                ?? GetInformationalVersion(assembly)
                ?? "(версия не найдена)";
        }
    }

    private static string? GetInformationalVersion(Assembly assembly)
        => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

    public string VersionSuffix
    {
        get
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string? informationalVersion = GetInformationalVersion(assembly);
            if (string.IsNullOrEmpty(informationalVersion))
                return "(версия не найдена)";

            // Разделяем основную версию и суффикс
            var parts = informationalVersion.Split('-', 2);
            string versionSuffix = parts.Length > 1 ? parts[1] : string.Empty;

            return versionSuffix;
        }
    }
}