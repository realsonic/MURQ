namespace MURQ.Application.Interfaces;
public interface IVersionProvider
{
    string VersionPrefix { get; }

    string VersionSuffix { get; }
}
