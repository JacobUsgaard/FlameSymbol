using Characters;

namespace Items.Weapons.Attackable.Strength.Axe
{
    public class IronAxe : Axe
    {

        private IronAxe() { }

        public static IronAxe Create()
        {
            return CreateInstance<IronAxe>(
                text: Instantiate(GameManager.IronAxeTextPrefab),
                uses: 30,
                rank: Proficiency.Rank.E,
                hitPercentage: 80,
                might: 8,
                criticalPercentage: 1,
                weight: 7,
                ranges: new int[] { 1 });
        }
    }
}