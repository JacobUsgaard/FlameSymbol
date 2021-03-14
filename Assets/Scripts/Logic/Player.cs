using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Logic
{
    public class Player : ManagedScriptableObject
    {
        public HashSet<Character> Characters { get; } = new HashSet<Character>();
        public Color Color { get; set; }
    }
}