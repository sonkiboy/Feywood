using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public EventHandler<HitboxUpdateEventArgs> handler;
    public GameObject TargetObject;

    private Collider collider;
    
    private Collider[] foundColliders = new Collider[0];

    public List<GameObject> TargetObjects = new List<GameObject>();

    [SerializeField] List<string> CollisionTags;

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

        foreach(Collider collider in foundColliders)
        {
            if (CollisionTags.Contains(collider.gameObject.tag))
            {
                TargetObjects.Add(collider.gameObject);
            }
        }

        OnTargetUpdate();
    }

    private void OnTriggerEnter(Collider collision)
    {
        

    }

    private void OnTriggerExit(Collider collision)
    {
        
        
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
