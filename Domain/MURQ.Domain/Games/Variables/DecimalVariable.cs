namespace MURQ.Domain.Games.Variables;

public record DecimalVariable(string Name, decimal Value) : Variable(Name);

