using UnityEngine;
using UnityEngine.UI;

public abstract class Item : ScriptableObject {

    public Text Text;
    public int UsesTotal;
    public int UsesRemaining;
    public static GameManager GameManager = GameManager.gameManager;
}