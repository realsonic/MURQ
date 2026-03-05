using MURQ.Domain.Quests.Locations;

namespace MURQ.Domain.Quests.QuestLines.SubstitutionTrees;

public record CodeNode(string Text, Location Location) : TreeNode(Location);

