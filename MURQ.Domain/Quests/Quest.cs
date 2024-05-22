using MURQ.Domain.Quests.Instructions;

using System.Collections.Immutable;

namespace MURQ.Domain.Quests;

public class Quest(IImmutableList<Instruction> instructions)
{
    public IImmutableList<Instruction> Instructions { get; } = instructions;
}