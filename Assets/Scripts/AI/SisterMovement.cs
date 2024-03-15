using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SisterMovement : MonoBehaviour
{
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    [SerializeField] Collider agentCollider;
    bool talkedTo = false;
    


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        //if (agentCollider.gameObject.CompareTag("Player"))
        //{
        //    //GotoNextPoint();
        //}
            
    }


    void GotoNextPoint()
    {
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
            talkedTo = true;
        }
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
            
            


    }

}



