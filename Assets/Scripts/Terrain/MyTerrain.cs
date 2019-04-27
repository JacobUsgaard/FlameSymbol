using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyTerrain : MonoBehaviour {
    public int hitPercentage = 0;
    public int defensePercentage = 0;
    public int movementCost = 1;
    public bool isPassable = true;
}
