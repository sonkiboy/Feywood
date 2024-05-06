#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using DialogueUI;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

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
                DialogueManager script = FindFirstObjectByType<DialogueManager>();
                SisterMovement sisterMovement = sisClone.GetComponentInChildren<SisterMovement>();
                script.onDialogueEnd2 = new UnityEvent();
                UnityAction methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), sisterMovement, "TalkedTo") as UnityAction;
                UnityEventTools.AddPersistentListener(script.onDialogueEnd2, methodDelegate);
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
#endif
