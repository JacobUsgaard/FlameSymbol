using System.Collections.Generic;
using UnityEngine;

public class ManagedMonoBehavior : MonoBehaviour
{

    public static GameManager GameManager { get; set; }

    public static void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;
    }

    public void DestroyAll(ICollection<Transform> transforms = default(List<Transform>))
    {
        foreach (Transform transform in transforms)
        {
            Destroy(transform.gameObject);
        }

        transforms.Clear();
    }
}
