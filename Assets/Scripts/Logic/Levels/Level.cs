using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : ScriptableObject
{
    public Character[,] CharacterMap;
    public MyTerrain[,] TerrainMap;
    public GameManager GameManager;
    public Player HumanPlayer;
    public Player AiPlayer;

    public Vector2 StartPosition;

    public void Init(GameManager gameManager, Player humanPlayer, Player aiPlayer)
    {
        GameManager = gameManager;
        HumanPlayer = humanPlayer;
        AiPlayer = aiPlayer;

        Init();

        if (TerrainMap.GetLength(0) != CharacterMap.GetLength(0) || TerrainMap.GetLength(1) != CharacterMap.GetLength(1))
        {
            Debug.LogError("Character map and terrain map must be the same dimensions");
            return;
        }

        for (int x = 0; x < TerrainMap.GetLength(0); x++)
        {
            for (int y = 0; y < TerrainMap.GetLength(1); y++)
            {
                TerrainMap[x, y].transform.position = new Vector2(x, y);

                Character character = CharacterMap[x, y];
                if (character != null)
                {
                    character.transform.position = new Vector2(x, y);
                }
            }
        }

        GameManager.Cursor.transform.position = StartPosition;
    }

    /// <summary>
    /// Check the board to see if the position is out of bounds.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsOutOfBounds(float x, float y)
    {
        return (x < 0f || x >= CharacterMap.GetLength(0) || y < 0f || y >= CharacterMap.GetLength(1) || !TerrainMap[(int)x,(int)y].isPassable);
    }

    /// <summary>
    /// Check the board to see if the position is out of bounds.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsOutOfBounds(Vector2 position)
    {
        return IsOutOfBounds(position.x, position.y);
    }

    /// <summary>
    /// Get the character at the given position.
    /// </summary>
    /// <param name="x">The x coordination of the position in question.</param>
    /// <param name="y">The y coordination of the position in question.</param>
    /// <returns>The character, if it exists</returns>
    public Character GetCharacter(float x, float y)
    {
        if (IsOutOfBounds(x, y))
        {
            return null;
        }

        return CharacterMap[(int)x, (int)y];
    }

    public Character GetCharacter(Vector3 position)
    {
        return GetCharacter(position.x, position.y);
    }

    /// <summary>
    /// Update the character map.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetCharacter(Character character, float x, float y)
    {
        if (IsOutOfBounds(x, y))
        {
            Debug.LogError("Position is out of bounds: (" + x + "," + y + ")");
        }

        CharacterMap[(int)x, (int)y] = character;
        if (character != null)
        {
            character.transform.position = new Vector2(x, y);
        }
    }

    /// <summary>
    /// Create the terrain and character maps. 
    /// The GameManager, HumanPlayer, and AiPlayer have all been initialized before this method is called.
    /// </summary>
    protected abstract void Init();
}