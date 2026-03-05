using System.Collections.Immutable;

using MURQ.Common.Exceptions;
using MURQ.Domain.Quests.QuestLines;
using MURQ.Domain.Quests.Statements;

namespace MURQ.Domain.Quests;

public class Quest(IEnumerable<Statement> statements, IEnumerable<QuestLine> questLines = null) //todo убрать null
{
    public IImmutableList<Statement> Statements { get; } = statements.ToImmutableList();

    public List<QuestLine> QuestLines { get; } = [.. questLines];

    public Statement? StartingStatement => Statements.Count > 0 ? Statements[0] : null;

    public Statement? GetNextStatement(Statement? currentStatement)
    {
        if (currentStatement is null) return null;

        int currentStatementIndex = Statements.IndexOf(currentStatement);

        if (currentStatementIndex == -1)
            throw new MurqException($"Инструкция {currentStatement} не принадлежит этому квесту.");

        int nextStatementIndex = currentStatementIndex + 1;

        return nextStatementIndex > MaxStatementIndex ? null : Statements[nextStatementIndex];
    }

    private int MaxStatementIndex => Statements.Count - 1;
}