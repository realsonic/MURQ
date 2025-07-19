using MURQ.Domain.Games;

namespace MURQ.Domain.Quests.UrqStrings;

/// <summary>
/// Представляет любую строку URQ, где возможны постановки
/// </summary>
public class UrqString(IEnumerable<UrqStringPart> parts)
{
    public static implicit operator UrqString(string text) => new([new UrqStringTextPart(text)]);

    public string ToString(IGameContext gameContext)
    {
        IEnumerable<string> partStrings = parts.Select(p => p.ToString(gameContext));
        return string.Join(string.Empty, partStrings);
    }
}
