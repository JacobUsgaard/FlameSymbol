using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : Item
{
    protected readonly HashSet<int> _ranges = new HashSet<int>();

    public Weapon(Text text, int uses, params int[] ranges) : base(text, uses)
    {
        if(ranges != null)
        {
            foreach(int range in ranges)
            {
                _ranges.Add(range);
            }
        }
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
