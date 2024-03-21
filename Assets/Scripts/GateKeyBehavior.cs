using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKeyBehavior : PickupBehavior
{
    public GameObject startPos;

    private void Start()
    {
    }

    public override void LoadData(GameData data)
    {
        
        if(DataManager.instance.Data.CurrentScene == "Intro_House")
        {
            Debug.Log($"Sending key at {transform.position} to: {startPos}");
            this.transform.position = startPos.transform.position;
        }

        

    }

    public override void SaveData(ref GameData data)
    {

    }
}
