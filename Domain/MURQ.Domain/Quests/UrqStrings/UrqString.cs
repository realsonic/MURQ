using MURQ.Domain.Games;

using System.Text;

namespace MURQ.Domain.Quests.UrqStrings;

/// <summary>
/// Представляет любую строку URQ, где возможны подстановки
/// </summary>
public class UrqString(IEnumerable<UrqStringPart> parts)
{
    public static implicit operator UrqString(string text) => new([new UrqStringTextPart(text)]);

    public string ToString(IGameContext gameContext)
    {
        StringBuilder stringBuilder = new();

        foreach (var part in parts)
        {
            stringBuilder.Append(part.ToString(gameContext));
        }
        
        return stringBuilder.ToString();
    }
}
