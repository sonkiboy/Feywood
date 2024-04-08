using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DialogueUI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class DadMovement : MonoBehaviour
{
    #region Object and Component Refrences

    // player object
    GameObject player;

    // represents where the dad can see
    BoxCollider visionCollider;

    // dad's rigidbody
    Rigidbody rb;

    private NavMeshAgent agent;

    #endregion

    public Transform[] points;
    private int destPoint = 0;
    
    float timer = 0;
    float waitTime = 3f;
    //bool waitToMove = true;

    private void Awake()
    {
        visionCollider = transform.Find("SightBounds").GetComponent<BoxCollider>();
    }
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
                    //Debug.Log("Waiting");
                    // add time.deltatime until no longer true
                    timer += Time.deltaTime;
                }
                else
                {
                    //Debug.Log("Moving");
                    GotoNextPoint();
                    timer = 0;
                //waitToMove= false;
                }

           
        }
        //else
        //{

        //}

        CheckSight();

    }

    void CheckSight()
    {
        //Debug.Log($"Vision collider: center {visionCollider.center} size: {visionCollider.size} rotation: {visionCollider.gameObject.transform.rotation}");
        Collider[] foundColliders = Physics.OverlapBox(visionCollider.bounds.center, visionCollider.size, visionCollider.gameObject.transform.rotation, visionCollider.includeLayers);
        
        
        //Debug.Log($"Found: {foundColliders.Length}");
        if (foundColliders.Length > 0)
        {
            if (foundColliders[0].gameObject.name == "Player")
            {
                Debug.Log($"Player in Dad's sight");
                PlayerController controller = foundColliders[0].gameObject.GetComponent<PlayerController>();

                if (controller.heldObject != null)
                {
                    if (controller.heldObject.name == "Key" && controller.IsHidden == false && controller.CurrentRoom == "Kitchen")
                    {
                        controller.heldObject = null;
                        CatchPlayer();
                    }
                }

            }
        }

    }

    

    // called when player is caught
    private void CatchPlayer()
    {
        Debug.Log("Dad caught Player!");

        this.GetComponent<DialogueTrigger>().TriggerDialogue();

        //player.GetComponent<PlayerController>().RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        DataManager.instance.SaveGame();


        DataManager.instance.Data.SpawnPointName = "HouseSpawn";
        DataManager.instance.Data.heldObj = null;


        DataManager.instance.LoadGame();
    }
}