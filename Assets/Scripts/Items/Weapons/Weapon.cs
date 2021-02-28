using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Items.Weapons
{
    /// <summary>
    /// Base weapon class
    /// </summary>
    public abstract class Weapon : Item
    {
        public int HitPercentage;
        public int Might;
        public int CriticalPercentage;
        public int Weight;
        public HashSet<int> Ranges { get; } = new HashSet<int>();
        public Proficiency.Rank RequiredProficiencyRank;

        /// <summary>
        /// Calculate the damage done by the attacking character to the defending character using this weapon
        /// Damage = Strength + Might - Defense
        /// </summary>
        /// <param name="attackingCharacter"></param>
        /// <param name="defendingCharacter"></param>
        /// <param name="defendingWeapon"></param>
        /// <returns></returns>
        public virtual int CalculateDamage(Character attackingCharacter, Character defendingCharacter, Weapon defendingWeapon)
        {
            return Might;
        }

        public override string ToString()
        {
            return string.Format("{0}=[HitPercentage: {1}, Might: {2}, CriticalPercentage: {3}, RequiredProficiencyRank: {4}, Weight: {5}]",
                Text.text,
                HitPercentage,
                Might,
                CriticalPercentage,
                RequiredProficiencyRank,
                Weight);
        }

        // TODO figure out how to do defaults for weapons better
        public static Type CreateInstance<Type>(Text text, int uses, Proficiency.Rank rank, int hitPercentage, int might, int criticalPercentage, int weight, params int[] ranges) where Type : Weapon
        {
            Type weapon = CreateInstance<Type>();
            weapon.Text = text;
            weapon.UsesTotal = uses;
            weapon.UsesRemaining = uses;
            weapon.RequiredProficiencyRank = rank;
            weapon.HitPercentage = hitPercentage;
            weapon.Might = might;
            weapon.CriticalPercentage = criticalPercentage;
            weapon.Weight = weight;

            if (ranges != null)
            {
                foreach (int range in ranges)
                {
                    weapon.Ranges.Add(range);
                }
            }

            return weapon;
        }

        public bool IsInRange(Vector2 start, Vector2 end)
        {
            foreach (int range in Ranges)
            {
                if (IsInRange(start.x, start.y, end.x, end.y, range))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInRange(float startX, float startY, float endX, float endY, int currentRange)
        {
            if (currentRange < 0)
            {
                return false;
            }

            if (currentRange == 0)
            {
                return startX == endX && startY == endY;
            }

            int newRange = currentRange - 1;
            return
                IsInRange(startX - 1, startY, endX, endY, newRange)
                ||
                IsInRange(startX + 1, startY, endX, endY, newRange)
                ||
                IsInRange(startX, startY - 1, endX, endY, newRange)
                ||
                IsInRange(startX, startY + 1, endX, endY, newRange);
        }
    }
}