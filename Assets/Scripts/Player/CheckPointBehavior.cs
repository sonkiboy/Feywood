using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehavior : MonoBehaviour
{


    [HideInInspector]
    public Vector3 SpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPosition = transform.Find("SpawnPos").position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the player enteres the trigger collider, then set their spawn position to this check points spawn position
        if(other.gameObject.name == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().RespawnPos = SpawnPosition;
        }
    }
}
