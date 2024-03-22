using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject player = other.gameObject;
            if(player.GetComponentInChildren<GateKeyBehavior>() != null)
            {
                Debug.Log("Gate Opened!");
                this.GetComponentInChildren<GameOverScreen>().Trigger();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
