using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : ScriptableObject {
    public readonly Character[,] CharacterMap;
    public readonly Transform[,] TerrainMap;
    public readonly GameManager GameManager;

    public Level(GameManager gameManager)
    {
        GameManager = gameManager;
    }

    public virtual void Draw()
    {
        for (int x = 0; x < TerrainMap.GetLength(0); x++)
        {
            for (int y = 0; y < TerrainMap.GetLength(1); y++)
            {
                Transform terrain = TerrainMap[x, y];
                if (terrain != null)
                {
                    Instantiate(terrain, new Vector3(x, y), Quaternion.identity, GameManager.transform);
                }
            }
        }

        for (int x = 0; x < CharacterMap.GetLength(0); x++)
        {
            for (int y = 0; y < CharacterMap.GetLength(1); y++)
            {
                Character character = CharacterMap[x, y];
                if (character != null)
                {
                    character.DrawCharacter(x, y);
                }
            }
        }
    }
}