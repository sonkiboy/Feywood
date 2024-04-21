using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour, IDataPersistance
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void LoadData(GameData data)
    {
        Vector3 pos = Vector3.zero;

        // if the object is found in the dictionary, store its value in pos and set the obj's position to pos
        if (DataManager.instance.Data.PickupsInScene.TryGetValue(this.gameObject, out pos))
        {
            this.gameObject.transform.position = pos;
        }
    }

    public virtual void SaveData(ref GameData data)
    {
        
    }
}
