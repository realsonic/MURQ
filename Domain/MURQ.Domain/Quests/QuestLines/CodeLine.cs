using MURQ.Domain.Games;
using MURQ.Domain.Games.Values;
using MURQ.Domain.Quests.Expressions;
using MURQ.Domain.Quests.QuestLines.SubstitutionTrees;
using MURQ.Domain.URQL.Lexing.CharacterEnumerableExtensions;
using MURQ.Domain.URQL.Locations;

namespace MURQ.Domain.Quests.QuestLines;

public record CodeLine(TreeNode[] Nodes, Location Location) : QuestLine(Location)
{
    public IEnumerable<OriginatedCharacter> ToCode(IGameContext gameContext)
    {
        foreach (OriginatedCharacter codeCharacter in ConvertNodesToCode(Nodes, gameContext))
            yield return codeCharacter;
    }

    private static IEnumerable<OriginatedCharacter> ConvertNodesToCode(TreeNode[] treeNodes, IGameContext gameContext)
    {
        foreach (TreeNode treeNode in treeNodes)
        {
            switch (treeNode)
            {
                case CodeNode codeNode:
                    foreach (PositionedCharacter sourceCharacter in codeNode.SourceCharacters)
                    {
                        yield return new OriginatedCharacter(sourceCharacter.Character, new PositionOrigin(sourceCharacter.Position));
                    }
                    break;

                case SubstitutionNode substitutionNode:
                    IEnumerable<OriginatedCharacter> sourceCharacters = ConvertNodesToCode(substitutionNode.Nodes, gameContext);
                    
                    string variableName = sourceCharacters.ToPlainString().Trim();

                    Value value = CalculateVariable(gameContext, variableName); //todo заменить на выражение

                    string stringValue = substitutionNode.Modifier switch
                    {
                        SubstitutionModifierEnum.None => value.AsDecimal.ToString(),
                        SubstitutionModifierEnum.AsString => value.AsString,
                        _ => throw new NotImplementedException($"Модификатор подстановки {substitutionNode.Modifier} пока не обрабатывается."),
                    };

                    foreach (char character in stringValue)
                    {
                        yield return new OriginatedCharacter(character, new LocationOrigin(substitutionNode.Location));
                    }

                    break;

                default:
                    throw new NotImplementedException($"Элемент дерева подстановок типа {treeNode.GetType()} пока не обрабатывается.");
            }
        }
    }

    private static Value CalculateVariable(IGameContext gameContext, string variableName)
    {
        VariableExpression variableExpression = new() { Name = variableName };
        return variableExpression.Calculate(gameContext);
    }
}