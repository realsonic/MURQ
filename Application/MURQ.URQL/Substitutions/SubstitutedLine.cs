using MURQ.URQL.Locations;

using System.Text.Json.Serialization;

using static MURQ.URQL.Substitutions.SubstitutedLine;
using static MURQ.URQL.Substitutions.SubstitutedLine.SubstitutionPart;

namespace MURQ.URQL.Substitutions;

public record SubstitutedLine(SubstitutedLinePart[] Parts)
{
    [JsonDerivedType(typeof(StringPart))]
    [JsonDerivedType(typeof(SubstitutionPart))]
    public abstract record SubstitutedLinePart(Location Location);
    public record StringPart(string Text, Location Location) : SubstitutedLinePart(Location);
    public record SubstitutionPart(SubstitutionModifierEnum Modifier, SubstitutedLinePart[] Parts, Location Location) : SubstitutedLinePart(Location)
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SubstitutionModifierEnum
        {
            None,
            AsString
        }
    }
}