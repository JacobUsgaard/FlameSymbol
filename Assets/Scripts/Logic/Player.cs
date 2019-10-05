using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ManagedScriptableObject
{
    private readonly HashSet<Character> _characters = new HashSet<Character>();

    public HashSet<Character> Characters
    {
        get
        {
            return _characters;
        }
    }

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
