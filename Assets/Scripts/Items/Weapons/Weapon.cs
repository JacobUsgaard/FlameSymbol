using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Items.Weapons
{
    public abstract class Weapon : Item
    {
        public int HitPercentage;
        public int Damage;
        public int CriticalPercentage;
        public HashSet<int> Ranges { get; } = new HashSet<int>();
        public Proficiency.Rank RequiredProficiencyRank;


        /// <summary>
        /// Calculate the damage done by the attacking character to the defending character using this weapon
        /// </summary>
        /// <param name="attackingCharacter"></param>
        /// <param name="defendingCharacter"></param>
        /// <returns></returns>
        public virtual int CalculateDamage(Character attackingCharacter, Character defendingCharacter)
        {
            return Damage;
        }

        public override string ToString()
        {
            return Text.text + "=[HitPercentage: " + HitPercentage + ", Damage: " + Damage + ", CriticalPercentage: " + CriticalPercentage + ", RequiredProficiencyRank: " + RequiredProficiencyRank + "]";
        }

        // TODO figure out how to do defaults for weapons better
        public static Type CreateInstance<Type>(Text text, int uses, Proficiency.Rank rank, int hitPercentage, int damage, int criticalPercentage, params int[] ranges) where Type : Weapon
        {
            Type weapon = CreateInstance<Type>();
            weapon.Text = text;
            weapon.UsesTotal = uses;
            weapon.UsesRemaining = uses;
            weapon.RequiredProficiencyRank = rank;
            weapon.HitPercentage = hitPercentage;
            weapon.Damage = damage;
            weapon.CriticalPercentage = criticalPercentage;

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