using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSword : Sword
{
    private IronSword() { }

    public static IronSword Create()
    {
        return CreateInstance<IronSword>(GameManager.IronSwordTextPrefab, 30, Character.Proficiency.Rank.E, 100, 5, 5, 1);
    }
}