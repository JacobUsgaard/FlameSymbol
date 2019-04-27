public class IronAxe : Axe {
    private IronAxe() { }

    public static IronAxe Create()
    {
        return CreateInstance<IronAxe>(GameManager.IronAxeTextPrefab, 30, Proficiency.E, 80, 8, 2, 1);
    }
}