using UnityEngine;

public class Proficiency
{
    public System.Type type;
    public Rank rank;
    public int experience;

    public Proficiency(System.Type type, Rank rank)
    {
        if (!type.IsSubclassOf(typeof(Weapon)))
        {
            Debug.LogErrorFormat("Created Proficiency for non-weapon: {0}", type);
        }
        else if (!type.IsAbstract)
        {
            Debug.LogErrorFormat("Created Proficiency for non-abstract: {0}", type);
        }
        this.type = type;
        this.rank = rank;
        experience = 0;
    }

    public void AddExperience(int experience)
    {
        if (experience > 100)
        {
            Debug.LogErrorFormat("Can't increase by more than 100: {0}", experience);
        }
        if (rank.Equals(Rank.S))
        {
            return;
        }

        this.experience += experience;

        if (this.experience >= 100)
        {
            rank += 1;
            this.experience %= 100;
        }
    }

    public override string ToString()
    {
        return string.Format("Proficiency:[Type: {0}, Rank: {1}, Rank: {2}]", type, rank, experience);
    }

    public enum Rank
    {
        E,
        D,
        C,
        B,
        A,
        S
    }

}
