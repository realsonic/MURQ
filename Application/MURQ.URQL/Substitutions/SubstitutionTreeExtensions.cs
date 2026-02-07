using MURQ.Domain.Games;
using MURQ.Domain.Quests.Expressions;
using MURQ.URQL.Locations;

using static MURQ.URQL.Substitutions.SubstitutionTree;
using static MURQ.URQL.Substitutions.SubstitutionTree.SubstitutionNode;

namespace MURQ.URQL.Substitutions;

public static class SubstitutionTreeExtensions
{
    public static IEnumerable<(char Character, Position Position)> ToRawUrql(this SubstitutionTree substitutionTree, IGameContext gameContext)
    {
        foreach ((char Character, Position Position) rawCharacter in NodesToRawUrql(substitutionTree.Nodes, gameContext))
            yield return rawCharacter;
    }

    private static IEnumerable<(char Character, Position Position)> NodesToRawUrql(Node[] nodes, IGameContext gameContext)
    {
        foreach (Node node in nodes)
        {
            switch (node)
            {
                case StringNode stringNode:
                    foreach (char character in stringNode.Text)
                    {
                        yield return (character, null); // todo возвращать позицию
                    }
                    break;

                case SubstitutionNode substitutionNode:
                    IEnumerable<(char Character, Position Position)> rawCharacters = NodesToRawUrql(substitutionNode.Nodes, gameContext);

                    string variableName = string.Join(string.Empty, rawCharacters.Select(rawCharacter => rawCharacter.Character));
                    VariableExpression variableExpression = new() { Name = variableName };
                    Domain.Games.Values.Value value = variableExpression.Calculate(gameContext);

                    switch(substitutionNode.Modifier)
                    {
                        case SubstitutionModifierEnum.None:
                            string numberAsString = value.AsDecimal.ToString();
                            foreach (char character in numberAsString)
                            {
                                yield return (character, null); // todo возвращать позицию
                            }
                            break;

                        case SubstitutionModifierEnum.AsString:
                            foreach (char character in value.AsString)
                            {
                                yield return (character, null); // todo возвращать позицию
                            }
                            break;

                        default: throw new NotImplementedException($"Модификатор подстановки {substitutionNode.Modifier} пока не обрабатывается.");
                    };
                    break;
                
                default:
                    throw new NotImplementedException($"Элемент дерева подстановок типа {node.GetType()} пока не обрабатывается.");
            }
        }
    }
}
