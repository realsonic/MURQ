using MURQ.Domain.Instructions;

namespace MURQ.Domain;

public class Quest(IReadOnlyCollection<Instruction> instructions)
{
    public IReadOnlyCollection<Instruction> Instructions { get; } = instructions;
}