using UnityEngine;

public class ManagedMonoBehavior : MonoBehaviour
{
    public static GameManager GameManager { get; set; }

    public static void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;
    }
}
