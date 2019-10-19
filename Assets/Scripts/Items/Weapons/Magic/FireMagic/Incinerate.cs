public class Incinerate : FireMagic {

    public static Incinerate Create()
    {
        return CreateInstance<Incinerate>(
            text: Instantiate(GameManager.IncinerateTextPrefab),
            uses: 20,
            rank: Proficiency.Rank.D,
            hitPercentage: 100,
            damage: 20,
            criticalPercentage: 3,
            ranges: new int[] { 1, 2 });
    }
}