using Items.Weapons;
using UnityEngine;

namespace Characters
{
    public class Proficiency
    {
        public enum Rank
        {
            E,
            D,
            C,
            B,
            A,
            S
        }

        public System.Type Type { get; set; }
        public Rank ProficiencyRank { get; set; }
        public int Experience { get; set; }

        public Proficiency(System.Type type, Rank rank)
        {
            if (!type.IsSubclassOf(typeof(Weapon)))
            {
                Debug.LogErrorFormat("Created Proficiency for non-weapon: {0}", type);
                return;
            }
            else if (!type.IsAbstract)
            {
                Debug.LogErrorFormat("Created Proficiency for non-abstract: {0}", type);
                return;
            }
            Type = type;
            ProficiencyRank = rank;
            Experience = 0;
        }

        public void AddExperience(int experience)
        {
            if (experience > 100)
            {
                Debug.LogErrorFormat("Can't increase by more than 100: {0}", experience);
            }

            Experience += experience;

            if (ProficiencyRank.Equals(Rank.S))
            {
                return;
            }

            if (Experience >= 100)
            {
                ProficiencyRank += 1;
                Experience %= 100;
            }
        }

        public override string ToString()
        {
            return string.Format("Proficiency:[Type: {0}, Rank: {1}, Experience: {2}]", Type, ProficiencyRank, Experience);
        }
    }
}