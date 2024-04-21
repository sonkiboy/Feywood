using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SisterSaveBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(GameData data)
    {
        if(DataManager.instance.Data.isSisterBackyard == true)
        {
            Destroy(this.gameObject);
        }
        if(DataManager.instance.Data.CurrentScene == "Intro_Backyard")
        {
            DataManager.instance.Data.isSisterBackyard = true;
        }
    }

    public void SaveData(ref GameData data)
    {
        
    }
}
