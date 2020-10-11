using System.Collections.Generic;

public class Player : ManagedScriptableObject
{
    public HashSet<Character> Characters { get; } = new HashSet<Character>();

    /// <summary>
    /// Find the first available enemy for attacking.
    /// TODO make this smarter so that it evaluates options a bit
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="attackRanges"></param>
    /// <returns></returns>
    public Character Attacks(float x, float y, HashSet<int> attackRanges)
    {
        foreach (int attackRange in attackRanges)
        {
            Character character = Attacks(x, y, attackRange);
            if (character != null)
            {
                return character;
            }
        }

        return null;
    }

    /// <summary>
    /// Find the first available enemy for attacking.
    /// TODO make this smarter so that it evaluates options a bit
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="attackRangeRemaining"></param>
    /// <returns></returns>
    public Character Attacks(float x, float y, int attackRangeRemaining)
    {
        if (attackRangeRemaining < 0 || GameManager.CurrentLevel.IsOutOfBounds(x, y))
        {
            return null;
        }

        Character character;
        if (attackRangeRemaining == 0)
        {
            character = GameManager.CurrentLevel.GetCharacter(x, y);
            if (character != null && !character.Player.Equals(this))
            {
                return character;
            }
        }

        character = Attacks(x - 1, y, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        character = Attacks(x + 1, y, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        character = Attacks(x, y - 1, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        character = Attacks(x, y + 1, attackRangeRemaining - 1);
        if (character != null)
        {
            return character;
        }

        return null;
    }
}
