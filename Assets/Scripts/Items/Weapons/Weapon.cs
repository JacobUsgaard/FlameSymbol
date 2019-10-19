using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : Item
{
    protected readonly HashSet<int> _ranges = new HashSet<int>();

    public int HitPercentage;
    public int Damage;
    public int CriticalPercentage;

    public Proficiency.Rank RequiredProficiencyRank;

    public virtual int CalculateDamage(Character defendingCharacter)
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

        if(ranges != null)
        {
            foreach(int range in ranges)
            {
                weapon._ranges.Add(range);
            }
        }

        return weapon;
    }

    public bool IsInRange(Vector3 start, Vector3 end)
    {
        foreach(int range in _ranges)
        {
            if(IsInRange(start.x, start.y, end.x, end.y, range))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsInRange(float startX, float startY, float endX, float endY, int currentRange)
    {
        if(currentRange < 0)
        {
            return false;
        }

        if(currentRange == 0)
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

    public HashSet<int> Ranges
    {
        get
        {
            return _ranges;
        }
    }
}
