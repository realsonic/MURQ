using MURQ.Domain.Quests.Locations;

namespace MURQ.Domain.Quests.QuestLines.SubstitutionTrees;

public record SubstitutionNode(SubstitutionModifierEnum Modifier, TreeNode[] Nodes, Location Location) : TreeNode(Location);

public enum SubstitutionModifierEnum
{
    None,
    AsString
}