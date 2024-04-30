using System.Collections;
using System.Collections.Generic;
using DialogueUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SisterMovement : MonoBehaviour
{
    public Transform[] points;
    [SerializeField] bool setActiveOnAwake = false;
    private int destPoint = 0;
    private NavMeshAgent agent;
    [SerializeField] Collider agentCollider;
    bool talkedTo = false;
    


    void Start()
    {
        agent = transform.parent.GetComponent<NavMeshAgent>();
        if(setActiveOnAwake)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;
          
    }

    //has agent go to next point in the array 
    void GotoNextPoint()
    {
        Debug.Log("Moving to Next Point");
        this.GetComponent<BoxCollider>().enabled = false;
        // Returns if no points have been set upw
        if (points.Length == 0)
            return;
        if (destPoint < points.Length)
            agent.destination = points[destPoint].position;
        else
            return;
        destPoint++;
    }

    void OnTriggerEnter(Collider agentcollider)
    {
        if(agentcollider.CompareTag("Player"))
        {
            if(this.GetComponent<DialogueTrigger>() != null)
            {
                this.GetComponent<DialogueTrigger>().TriggerDialogue();
                Destroy(this.GetComponent<DialogueTrigger>());
                
            }
            
        }
    }
    //sets talkedto to true
    public void TalkedTo()
    {
        talkedTo = true;
    }

    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (talkedTo)
        {
            GotoNextPoint();
            talkedTo = false;
        }
        //Check if the agent is near the desitination and re enables the collider
        if((points[destPoint].position - this.transform.position).magnitude < 50f)
        {
            this.GetComponent<BoxCollider>().enabled = true;
        }
    }

}



