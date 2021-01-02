public class Heal : HealingStaff
{
    public static Heal Create()
    {
        return CreateInstance<Heal>(
            text: Instantiate(GameManager.HealTextPrefab),
            uses: 30,
            rank: Proficiency.Rank.E,
            damage: 10,
            hitPercentage: 100,
            criticalPercentage: 0,
            ranges: 1);
    }

    public override void Assist(Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.Heal(Damage);
    }
}
