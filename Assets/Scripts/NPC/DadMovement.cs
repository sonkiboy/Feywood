using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DialogueUI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class DadMovement : MonoBehaviour, IDataPersistance
{
    #region Object and Component Refrences

    // player object
    GameObject player;

    // represents where the dad can see
    BoxCollider visionCollider;

    // dad's rigidbody
    Rigidbody rb;

    private NavMeshAgent agent;

    SneakUI sneakUI;


    #endregion

    public float CatchDelay = 1;

    public Transform[] points;
    private int destPoint = 0;

    float timer = 0;
    float waitTime = 3f;
    float lerpDuration = 1f;
    bool isRotating = false;
    //bool waitToMove = true;

    private void Awake()
    {
        visionCollider = transform.Find("SightBounds").GetComponent<BoxCollider>();
        sneakUI = GameObject.FindAnyObjectByType<SneakUI>();
    }

    public void LoadData(GameData data)
    {
        StartCoroutine(StartHunting());
    }

    public void SaveData(ref GameData data)
    {

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
            if(destPoint == 0)
            {
                //Debug.Log("At Kitchen Table");
                if (!isRotating)
                {
                    //Debug.Log("Coroutine");
                    StartCoroutine(RotateNPC());
                }
            }
            else
            {
                //Debug.Log("Stop Rotation");
                isRotating = false;
            }
            // if timer is less than the set wait time
            if (timer < waitTime)
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

        

    }

    IEnumerator RotateNPC()
    {
        Debug.Log("Rotating");
        isRotating = true;
        float timeElapsed = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 110, 0);

        while (timeElapsed < lerpDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
        yield return new WaitForSeconds(3f);
        isRotating = false;
        //this.transform.Rotate(0, 110, 0);
    }

    IEnumerator StartHunting()
    {
        while (true)
        {
            int count = 0;
            // if the player is in line of sight...
            while (CheckSight())
            {

                sneakUI.CurrentState = SneakUI.SneakStates.Spotted;

                if(count > CatchDelay * 60)
                {
                    CatchPlayer();
                    break;
                }

                yield return null;
                count++;
            }

            if(sneakUI.CurrentState == SneakUI.SneakStates.Spotted)
            {
                sneakUI.CurrentState = SneakUI.SneakStates.Exposed;
            }
            

            yield return null;
        }
    }


    bool CheckSight()
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
                        
                        return true;
                    }
                }

            }

        }
       
        return false;

    }



    // called when player is caught
    private void CatchPlayer()
    {
        StopCoroutine(StartHunting());

        GameObject.FindAnyObjectByType<PlayerController>().heldObject = null;

        Debug.Log("Dad caught Player!");

        this.GetComponent<DialogueTrigger>().TriggerDialogue();

        //player.GetComponent<PlayerController>().RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        DataManager.instance.SaveGame();


        DataManager.instance.Data.SpawnPointName = "HouseSpawn";
        DataManager.instance.Data.heldObj = null;


        StartCoroutine(DataManager.instance.HintLoad(3f, GetComponent<GameOverScreen>()));
    }
}