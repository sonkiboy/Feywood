using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    //creating an end goal for the NPC to move to
    public Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        //connecting the Transform goal to the agent's end pos
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }

   
    
}
