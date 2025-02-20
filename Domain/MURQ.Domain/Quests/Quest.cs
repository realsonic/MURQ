using System.Collections.Immutable;

using MURQ.Domain.Exceptions;
using MURQ.Domain.Quests.Instructions;

namespace MURQ.Domain.Quests;

public class Quest(IImmutableList<Instruction> instructions)
{
    public IImmutableList<Instruction> Instructions { get; } = instructions;

    public Instruction? FirstInstruction => Instructions.Count > 0 ? Instructions[0] : null;

    public Instruction? GetNextInstruction(Instruction? currentInstruction)
    {
        if (currentInstruction is null) return null;

        int currentInstructionIndex = Instructions.IndexOf(currentInstruction);

        if (currentInstructionIndex == -1)
            throw new MurqException($"Инструкция {currentInstruction} не принадлежит этому квесту.");

        int nextInstructionIndex = currentInstructionIndex + 1;

        return nextInstructionIndex > MaxInstructionIndex ? null : Instructions[nextInstructionIndex];
    }

    private int MaxInstructionIndex => Instructions.Count - 1;
}