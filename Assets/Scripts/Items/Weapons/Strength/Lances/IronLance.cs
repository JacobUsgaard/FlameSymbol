public class IronLance : Lance {

    private IronLance() { }

    public static IronLance Create()
    {
        return CreateInstance<IronLance>(
            text: Instantiate(GameManager.IronAxeTextPrefab),
            uses: 30,
            rank: Proficiency.Rank.E,
            hitPercentage: 90,
            damage: 6,
            criticalPercentage: 5,
            ranges: new int[] { 1 });
    }
}
