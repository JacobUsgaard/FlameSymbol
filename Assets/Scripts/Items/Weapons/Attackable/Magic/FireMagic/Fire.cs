using Characters;

namespace Items.Weapons.Attackable.Magic.FireMagic
{
    public class Fire : FireMagic
    {
        private Fire() { }

        public static Fire Create()
        {
            return CreateInstance<Fire>(
                text: Instantiate(GameManager.FireTextPrefab),
                uses: 30,
                rank: Proficiency.Rank.E,
                hitPercentage: 100,
                might: 10,
                criticalPercentage: 3,
                weight: 6,
                ranges: new int[] { 1, 2 });
        }
    }
}