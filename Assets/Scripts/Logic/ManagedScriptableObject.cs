using UnityEngine;
using System.Collections.Generic;

public class ManagedScriptableObject : ScriptableObject
{

    public static GameManager GameManager { get; set; }

    public static void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;
    }

    protected void DeleteAll(ICollection<Transform> collection)
    {
        foreach(Transform t in collection)
        {
            Destroy(t.gameObject);
        }

        collection.Clear();
    }
}
