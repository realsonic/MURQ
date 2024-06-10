using System.Collections.Immutable;

using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Quests;

public class Quest(IImmutableList<Instruction> instructions)
{
    public IImmutableList<Instruction> Instructions { get; } = instructions;

    public int? GetInstructionIndex(Instruction instruction)
    {
        int instructionIndex = Instructions.IndexOf(instruction, 0, 1, null);
        return instructionIndex >= 0 ? instructionIndex : null;
    }
}