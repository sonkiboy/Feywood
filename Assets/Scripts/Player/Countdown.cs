using System.Collections;
using System.Collections.Generic;
using DialogueUI;
using Unity.VisualScripting;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] GameObject Sis;
    [SerializeField] Transform sisSpawn;
    PlayerController controller;
    public ScreenTransition transition;
    private GameObject sisClone;
    private GameObject enableThis;
    private Vector3 lookDir = new Vector3(1, 0, 0);
    private Vector3 lookDir2 = new Vector3(0, 0, 0);
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
                
                Sis.GetComponent<DialogueTrigger>().enabled = true;
                
                Sis.transform.GetChild(0).GetComponent<SisterMovement>().enabled = true;
                sisClone=Instantiate(Sis, sisSpawn.transform.position, Quaternion.LookRotation(lookDir,lookDir2));
                //enableThis = transform.Find("HideAndSeekPart3").gameObject;
                Destroy(Sis);
                transition.In(() =>
                {
                    controller.currentRestriction = PlayerController.MovementRestrictions.None;
                });
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
