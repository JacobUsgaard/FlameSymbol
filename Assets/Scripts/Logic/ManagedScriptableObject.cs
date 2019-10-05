using UnityEngine;

public class ManagedScriptableObject : ScriptableObject {

    public static GameManager GameManager { get; set; }

    public static void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;
    }
}
