using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public EventHandler<HitboxUpdateEventArgs> handler;
    public GameObject TargetObject;
    
    public List<GameObject> TargetObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log($"{this.name} collided with {collision.gameObject.name}");
        if(collision.gameObject.tag == "Pickup" || collision.gameObject.tag == "Moveable")
        TargetObjects.Add(collision.gameObject);


        OnTargetUpdate();

    }

    private void OnTriggerExit(Collider collision)
    {
        if (TargetObjects.Contains(collision.gameObject))
        {
            TargetObjects.Remove(collision.gameObject);
        }
        

        

        OnTargetUpdate();
        
    }

    void OnTargetUpdate()
    {
        HitboxUpdateEventArgs arg = new HitboxUpdateEventArgs();

        if (TargetObjects.Count >= 1)
        {
            TargetObject = TargetObjects[0];
            //Debug.Log($"Updating {this.name}'s target to {TargetObject.name}");
        }
        else
        {
            TargetObject = null;
            //Debug.Log($"Updating {this.name}'s target to null");
        }

        arg.targetObject = TargetObject;

        if (handler != null)
        {
            handler(this,arg);
        }
    }
}

public class HitboxUpdateEventArgs : EventArgs
{
    public GameObject targetObject;
}
