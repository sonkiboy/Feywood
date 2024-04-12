using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] GameObject Sis;
    [SerializeField] Transform sisSpawn;
    PlayerController controller;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision Counting)
    {
        if (Counting.gameObject.name.Equals("Player"))
        {
            controller.currentRestriction = PlayerController.MovementRestrictions.noMovement;
            timeElapse();
        }

    }

    IEnumerator timeElapse()
    {
        yield return new WaitForSeconds(3);
        Destroy(Sis);
        Instantiate(Sis,sisSpawn.transform.position, Quaternion.identity);
    }
}
