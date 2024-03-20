using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public string CurrentScene;
    public string SpawnPointName;
    public GameObject heldObj;

    public bool isGateOpen;

    public GameData()
    {
        CurrentScene = "Intro_House";
        SpawnPointName = "HouseSpawn";
        heldObj = null;

        isGateOpen = false;
    }
}
