using UnityEngine;
using System.Collections.Generic;

namespace Logic
{
    public class ManagedScriptableObject : ScriptableObject
    {
        public static GameManager GameManager { get; set; }

        public static void Initialize(GameManager gameManager)
        {
            GameManager = gameManager;
        }
    }
}