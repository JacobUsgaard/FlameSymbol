using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : Level {

    /// <summary>
    /// Initialize the terrain map and characters. Should only be called by Level.Init(GameManager,Player,Player)
    /// </summary>
    protected override void Init()
    {
        
        TerrainMap = new MyTerrain[4, 4];
        CharacterMap = new Character[4, 4];

        TerrainMap[0, 0] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[0, 1] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[0, 2] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[0, 3] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[1, 0] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[1, 1] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[1, 2] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[1, 3] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[2, 0] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[2, 1] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[2, 2] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[2, 3] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[3, 0] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[3, 1] = Create<MyTerrain>(GameManager.GrassTerrain);
        TerrainMap[3, 2] = Create<MyTerrain>(GameManager.WallTerrain);
        TerrainMap[3, 3] = Create<MyTerrain>(GameManager.GrassTerrain);

        CharacterMap[0, 1] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[0, 1].Player = AiPlayer;
        CharacterMap[0, 1].Items.Add(IronSword.Create());

        CharacterMap[1, 2] = Create<Character>(GameManager.WizardPrefab);
        CharacterMap[1, 2].Player = AiPlayer;
        CharacterMap[1, 2].Items.Add(IronSword.Create());
        CharacterMap[1, 2].Items.Add(Incinerate.Create());

        CharacterMap[2, 2] = Create<Character>(GameManager.WizardPrefab);
        CharacterMap[2, 2].Player = HumanPlayer;
        CharacterMap[2, 2].Items.Add(IronSword.Create());
        CharacterMap[2, 2].Items.Add(Fire.Create());

        CharacterMap[3, 3] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[3, 3].Player = HumanPlayer;
        CharacterMap[3, 3].Items.Add(IronAxe.Create());

        StartPosition = new Vector2(0, 0);
    }

    protected Character Create<Character> (Transform original)
    {
        Component copy = Instantiate(original, GameManager.transform);
        return copy.GetComponent<Character>();
    }
}
