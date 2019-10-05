public class Fire : FireMagic {
    private Fire() { }

    public static Fire Create()
    {
        return CreateInstance<Fire>(GameManager.FireTextPrefab, 30, Character.Proficiency.Rank.E, 100, 10, 3, 1, 2);
    }
}