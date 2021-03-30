using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Logic
{
    public class Player : ManagedScriptableObject
    {
        public HashSet<Character> Characters { get; } = new HashSet<Character>();
        public Color Color { get; set; }
        public bool IsHuman { get; set; }
        public string Name { get; set; }

        public Player()
        {
            Name = GetType().Name;
        }

        public bool HaveAllCharactersMoved()
        {
            foreach (Character character in Characters)
            {
                if (!character.HasMoved)
                {
                    return false;
                }
            }

            return true;
        }
    }
}