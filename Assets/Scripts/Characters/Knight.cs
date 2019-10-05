using UnityEngine;

public class Knight : Character
{
    private Knight() { }
    void Start()
    {
        Debug.Log("Creating Knight");
        AddProficiency(new Proficiency(typeof(Sword), Proficiency.Rank.E));
        AddProficiency(new Proficiency(typeof(Lance), Proficiency.Rank.E));
        AddProficiency(new Proficiency(typeof(Axe), Proficiency.Rank.E));
    }
}