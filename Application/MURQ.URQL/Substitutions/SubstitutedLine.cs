namespace MURQ.URQL.Substitutions;
public abstract record SubstitutedLine;

public record StringLine(string Text) : SubstitutedLine;
