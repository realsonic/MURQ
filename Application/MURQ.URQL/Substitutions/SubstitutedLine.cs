using System.Text.Json.Serialization;

using static MURQ.URQL.Substitutions.SubstitutedLine;
using static MURQ.URQL.Substitutions.SubstitutedLine.SubstitutionPart;

namespace MURQ.URQL.Substitutions;

public record SubstitutedLine(SubstitutedLinePart[] Parts)
{
    [JsonDerivedType(typeof(StringPart))]
    [JsonDerivedType(typeof(SubstitutionPart))]
    public abstract record SubstitutedLinePart;
    public record StringPart(string Text) : SubstitutedLinePart;
    public record SubstitutionPart(SubstitutionModifierEnum Modifier, SubstitutedLinePart[] Parts) : SubstitutedLinePart
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SubstitutionModifierEnum
        {
            None,
            AsString
        }
    }
}