using Characters;

namespace Items.Weapons.Attackable.Strength.Sword
{
    public class IronSword : Sword
    {
        private IronSword() { }

        public static IronSword Create()
        {
            return CreateInstance<IronSword>(
                text: Instantiate(GameManager.IronSwordTextPrefab),
                uses: 30,
                rank: Proficiency.Rank.E,
                hitPercentage: 100,
                damage: 5,
                criticalPercentage: 5,
                ranges: new int[] { 1 });
        }
    }
}