using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : ScriptableObject {

    private HashSet<CharacterScript> characters = new HashSet<CharacterScript>();

    public GameManagerScript GameManager;

    public HashSet<CharacterScript> Characters
    {
        get
        {
            return characters;
        }

        set
        {
            characters = value;
        }
    }

    public void BeginTurn()
    {
        foreach(CharacterScript character in Characters)
        {
            character.HasMoved = false;
        }
    }

    public static PlayerScript CreateInstance(GameManagerScript gameManager)
    {
        PlayerScript player =  CreateInstance<PlayerScript>();
        player.GameManager = gameManager;

        return player;
    }

    public void EndTurn()
    {

    }
}
