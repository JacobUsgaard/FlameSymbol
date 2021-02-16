using Items.Weapons.Attackable.Strength.Axe;
using Items.Weapons.Attackable.Strength.Lance;
using Items.Weapons.Attackable.Strength.Sword;
using UnityEngine;

namespace Characters
{
    public class Knight : Character
    {
        void Start()
        {
            Debug.Log("Creating Knight");
            AddProficiency(new Proficiency(typeof(Sword), Proficiency.Rank.E));
            AddProficiency(new Proficiency(typeof(Lance), Proficiency.Rank.E));
            AddProficiency(new Proficiency(typeof(Axe), Proficiency.Rank.E));
        }
    }
}