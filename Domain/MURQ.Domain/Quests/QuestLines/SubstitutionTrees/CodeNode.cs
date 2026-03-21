using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.Quests.QuestLines.SubstitutionTrees;

public record CodeNode(List<PositionedCharacter> SourceCharacters, Location Location) : TreeNode(Location);

