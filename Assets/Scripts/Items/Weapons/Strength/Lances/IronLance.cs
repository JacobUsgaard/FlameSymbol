using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronLance : Lance {

    private IronLance() { }

    public static Lance Create()
    {
        return CreateInstance<Lance>(GameManager.IronLanceTextPrefab, 30, Character.Proficiency.Rank.E, 100, 6, 5, 1);
    }
}
