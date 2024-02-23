using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public EventHandler<HitboxUpdateEventArgs> handler;
    public GameObject TargetObject;
    
    public List<GameObject> TargetObjects = new List<GameObject>();
    [SerializeField] Collider[] foundColliders = new Collider[0];

    [SerializeField] List<string> CollisionTags;

    Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        foundColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.size);

        TargetObjects.Clear();

        foreach (Collider collider in foundColliders)
        {
            //Debug.Log($"{this.name} collided with {collision.gameObject.name}");
            if (CollisionTags.Contains(collider.gameObject.tag))
            {
                TargetObjects.Add(collider.gameObject);
            }
        }

        
        OnTargetUpdate();
        
    }



    private void OnTriggerEnter(Collider collision)
    {
        ////Debug.Log($"{this.name} collided with {collision.gameObject.name}");
        //if (CollisionTags.Contains(collision.gameObject.tag))
        //{
        //    TargetObjects.Add(collision.gameObject);
        //}
        


        //OnTargetUpdate();

    }

    private void OnTriggerExit(Collider collision)
    {
        //if (TargetObjects.Contains(collision.gameObject))
        //{
        //    TargetObjects.Remove(collision.gameObject);
        //}
        

        

        //OnTargetUpdate();
        
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
