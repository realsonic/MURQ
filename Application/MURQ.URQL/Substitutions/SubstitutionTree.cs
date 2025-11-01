using MURQ.URQL.Locations;

using System.Text.Json.Serialization;

using static MURQ.URQL.Substitutions.SubstitutionTree;
using static MURQ.URQL.Substitutions.SubstitutionTree.SubstitutionNode;

namespace MURQ.URQL.Substitutions;

public record SubstitutionTree(Node[] Nodes)
{
    [JsonDerivedType(typeof(StringNode))]
    [JsonDerivedType(typeof(SubstitutionNode))]
    public abstract record Node(Location Location);
    public record StringNode(string Text, Location Location) : Node(Location);
    public record SubstitutionNode(SubstitutionModifierEnum Modifier, Node[] Nodes, Location Location) : Node(Location)
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SubstitutionModifierEnum
        {
            None,
            AsString
        }
    }
}