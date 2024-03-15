using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DadMovement : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    float timer = 0;
    float waitTime = 3f;
    //bool waitToMove = true;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        //GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.005f)
        {
           
                // if timer is less than the set wiat time
                if(timer < waitTime)
                {
                    Debug.Log("Waiting");
                    // add time.deltatime until no longer true
                    timer += Time.deltaTime;
                }
                else
                {
                    Debug.Log("Moving");
                    GotoNextPoint();
                    timer = 0;
                //waitToMove= false;
                }

           
        }
        //else
        //{
            
        //}

    }
}