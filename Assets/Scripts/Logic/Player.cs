using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ScriptableObject {
    public HashSet<Character> Characters = new HashSet<Character>();
    public GameManager GameManager;
}
