using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAnimator : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"Sister Velocity = {Vector3.Dot(transform.TransformDirection(Vector3.forward), agent.velocity)}. Direction {transform.TransformDirection(Vector3.forward)} | Velocity: {agent.velocity}");
        animator.SetFloat("Speed", agent.velocity.x);
    }
}
