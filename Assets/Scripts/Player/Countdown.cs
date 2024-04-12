using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] GameObject Sis;
    [SerializeField] Transform sisSpawn;
    PlayerController controller;
    public ScreenTransition transition;

    private void Start()
    {
        controller = FindFirstObjectByType<PlayerController>();
    }


    private void OnTriggerEnter(Collider Counting)
    {
        if (Counting.gameObject.tag == "Player")
        {
            controller.currentRestriction = PlayerController.MovementRestrictions.noMovement;
            // timeElapse();
            transition.Out(
            () =>
            {

                Destroy(Sis);
                Instantiate(Sis, sisSpawn.transform.position, Quaternion.identity);
                transition.In();
            }
            );

            Debug.Log("fade");
        }
        Debug.Log("collided");
    }

    IEnumerator timeElapse()
    {
         /*new WaitForSeconds(3);*/
        
        yield return null;
    }
}
