using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKeyBehavior : PickupBehavior
{
    public GameObject startPos;
    private Rigidbody rb;

    private void Start()
    {
        startPos = GameObject.Find("KeySpawnPos");
        rb = GetComponent<Rigidbody>();
    }

    public override void LoadData(GameData data)
    {
        
        if(DataManager.instance.Data.CurrentScene == "Intro_House" && DataManager.instance.Data.heldObj != this.gameObject)
        {
            Debug.Log($"Sending key at {transform.position} to: {startPos}");
            this.transform.position = startPos.transform.position;
            GetComponent<Rigidbody>().isKinematic = true;
            transform.rotation = Quaternion.identity;
        }

        

    }

    public override void SaveData(ref GameData data)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.isKinematic = false;
        }
    }
}
