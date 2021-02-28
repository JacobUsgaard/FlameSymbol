using Characters;

namespace Items.Weapons.Attackable.Strength.Lance
{
    public class IronLance : Lance
    {
        private IronLance() { }

        public static IronLance Create()
        {
            return CreateInstance<IronLance>(
                text: Instantiate(GameManager.IronAxeTextPrefab),
                uses: 30,
                rank: Proficiency.Rank.E,
                hitPercentage: 90,
                might: 6,
                criticalPercentage: 5,
                weight: 6,
                ranges: new int[] { 1 });
        }
    }
}