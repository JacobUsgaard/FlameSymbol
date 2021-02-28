using Characters;

namespace Items.Weapons.Assistable.HealingStaffs
{
    public class Heal : HealingStaff
    {
        public static Heal Create()
        {
            return CreateInstance<Heal>(
                text: Instantiate(GameManager.HealTextPrefab),
                uses: 30,
                rank: Proficiency.Rank.E,
                might: 10,
                hitPercentage: 100,
                criticalPercentage: 0,
                weight: 4,
                ranges: 1);
        }

        public override void Assist(Character sourceCharacter, Character targetCharacter)
        {
            targetCharacter.Heal(Might);
        }
    }
}