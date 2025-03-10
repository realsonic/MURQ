﻿namespace MURQ.URQL.Lexers;
public static class CharExtensions
{
    public static bool IsEqualIgnoreCase(this char character, char otherCharacter) => char.ToLowerInvariant(character) == char.ToLowerInvariant(otherCharacter);
}
