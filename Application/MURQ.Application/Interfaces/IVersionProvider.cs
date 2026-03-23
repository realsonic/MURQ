namespace MURQ.Application.Interfaces;
public interface IVersionProvider
{
    string Version { get; }

    string VersionSuffix { get; }
}
