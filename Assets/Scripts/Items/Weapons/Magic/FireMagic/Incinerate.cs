using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerate : FireMagic {

    public static Incinerate Create()
    {
        return CreateInstance<Incinerate>(GameManager.IncinerateTextPrefab, 30, Proficiency.E, 100, 20, 3, 1, 2);
    }
}