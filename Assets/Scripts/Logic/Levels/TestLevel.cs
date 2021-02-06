using UnityEngine;

public class TestLevel : Level
{
    public override bool IsLevelOver()
    {
        return HumanPlayer.Characters.Count == 0 || AiPlayer.Characters.Count == 0;
    }

    /// <summary>
    /// Initialize the terrain map and characters. Should only be called by Level.Init(GameManager,Player,Player)
    /// </summary>
    protected override void Init()
    {
        TerrainMap = new Terrain.Terrain[4, 4];
        CharacterMap = new Character[4, 4];

        TerrainMap[0, 0] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[0, 1] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[0, 2] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[0, 3] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[1, 0] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[1, 1] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[1, 2] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[1, 3] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[2, 0] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[2, 1] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[2, 2] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[2, 3] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[3, 0] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[3, 1] = Create<Terrain.Terrain>(GameManager.GrassTerrain);
        TerrainMap[3, 2] = Create<Terrain.Terrain>(GameManager.WallTerrain);
        TerrainMap[3, 3] = Create<Terrain.Terrain>(GameManager.ForrestTerrain);

        CharacterMap[0, 0] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[0, 0].Player = AiPlayer;
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].Items.Add(IronSword.Create());
        CharacterMap[0, 0].AddProficiency(new Proficiency(typeof(Sword), Proficiency.Rank.E));

        CharacterMap[0, 1] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[0, 1].Player = AiPlayer;
        CharacterMap[0, 1].Items.Add(IronSword.Create());

        CharacterMap[0, 2] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[0, 2].Player = AiPlayer;
        CharacterMap[0, 2].Items.Add(IronSword.Create());

        CharacterMap[0, 3] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[0, 3].Player = AiPlayer;
        CharacterMap[0, 3].Items.Add(IronSword.Create());

        CharacterMap[2, 3] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[2, 3].Player = AiPlayer;
        CharacterMap[2, 3].Items.Add(IronSword.Create());

        CharacterMap[1, 2] = Create<Character>(GameManager.WizardPrefab);
        CharacterMap[1, 2].Player = AiPlayer;
        CharacterMap[1, 2].Items.Add(IronSword.Create());
        CharacterMap[1, 2].Items.Add(Incinerate.Create());

        CharacterMap[2, 2] = Create<Character>(GameManager.WizardPrefab);
        CharacterMap[2, 2].Player = HumanPlayer;
        CharacterMap[2, 2].Items.Add(IronSword.Create());
        CharacterMap[2, 2].Items.Add(Fire.Create());
        CharacterMap[2, 2].AddProficiency(new Proficiency(typeof(FireMagic), Proficiency.Rank.E));
        CharacterMap[2, 2].AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.E));

        CharacterMap[2, 1] = Create<Character>(GameManager.WizardPrefab);
        CharacterMap[2, 1].Player = HumanPlayer;
        CharacterMap[2, 1].CurrentHp = 10;
        CharacterMap[2, 1].AddProficiency(new Proficiency(typeof(HealingStaff), Proficiency.Rank.E));
        CharacterMap[2, 1].Items.Add(Fire.Create());
        CharacterMap[2, 1].AddProficiency(new Proficiency(typeof(FireMagic), Proficiency.Rank.E));

        CharacterMap[3, 3] = Create<Character>(GameManager.KnightPrefab);
        CharacterMap[3, 3].Player = AiPlayer;
        CharacterMap[3, 3].Items.Add(IronAxe.Create());

        StartPosition = new Vector2(0, 0);
    }
}
