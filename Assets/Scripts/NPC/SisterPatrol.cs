using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DialogueUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class SisterPatrol : MonoBehaviour
{
    #region Object and Component Refrences

    // player object
    GameObject player;

    // represents where the dad can see
    BoxCollider visionCollider;

    // dad's rigidbody
    Rigidbody rb;

    public float CatchDelay = 1;
    SneakUI sneakUI;

    private NavMeshAgent agent;

    #endregion

    public Transform[] points;
    private int destPoint = 0;

    public GameObject moveGO;
    bool isRotating = false;
    bool caught = false;
    float lerpDuration = 1f;
    float timer = 0;
    float waitTime = 5f;
    //bool waitToMove = true;
    public bool doneLooking = false;

    

    private void Awake()
    {
        visionCollider = transform.Find("SightBounds").GetComponent<BoxCollider>();
        sneakUI = GameObject.FindAnyObjectByType<SneakUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(StartHunting());
    }

    public void LoadData(GameData data)
    {

    }

    public void SaveData(ref GameData data)
    {

    }
    void Start()
    {
        agent = transform.parent.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");

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
        destPoint = (destPoint + 1);
        if (destPoint == 4)
        {
            doneLooking = true;
        }
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (doneLooking)
        {
            StopAllCoroutines();
            this.enabled = false;
            moveGO.SetActive(true);
            if (!caught)
            {
                this.GetComponent<DialogueTrigger>().TriggerDialogue();
            }
        }
        if (!agent.pathPending && agent.remainingDistance < 0.005f)
        {
            if (destPoint != 0)
            {
                Debug.Log("Not at start");
                if (!isRotating)
                {
                    Debug.Log("Coroutine");
                    StartCoroutine(RotateNPC());
                }
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
                Debug.Log("Moving");
                GotoNextPoint();
                timer = 0;
            }


        }
    }

    IEnumerator RotateNPC()
    {
        Debug.Log("Rotating");
        isRotating = true;
        float timeElapsed = 0;
        Quaternion startRotation = transform.parent.rotation;
        Quaternion targetRotation = transform.parent.rotation * Quaternion.Euler(0, 110, 0);

        while (timeElapsed < lerpDuration)
        {
            transform.parent.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.parent.rotation = targetRotation;
        yield return new WaitForSeconds(1.5f);
        isRotating = false;
        //this.transform.Rotate(0, 110, 0);
    }

    IEnumerator StartHunting()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            int count = 0;
            // if the player is in line of sight...
            while (CheckSight())
            {
                Debug.Log(count);
                sneakUI.CurrentState = SneakUI.SneakStates.Spotted;

                if (count > CatchDelay * 60)
                {
                    CatchPlayer();
                    break;
                }

                yield return null;
                count++;
            }

            if (sneakUI.CurrentState == SneakUI.SneakStates.Spotted)
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
                Debug.Log($"Player in Sister's sight");
                PlayerController controller = foundColliders[0].gameObject.GetComponent<PlayerController>();
                if (controller.IsHidden == false)
                {

                    return true;
                }
            }

        }

        return false;

    }

    // called when player is caught
    private void CatchPlayer()
    {
        Debug.Log("Sister caught Player!");

        caught = true;
        StopCoroutine(StartHunting());
        Destroy(this.GetComponent<DialogueTrigger>());
        destPoint = 3;
        GotoNextPoint();

        transform.Find("SightBounds").GetComponent<DialogueTrigger>().TriggerDialogue();

        DataManager.instance.SaveGame();


        DataManager.instance.Data.SpawnPointName = "H&Spawn";
        DataManager.instance.Data.heldObj = null;


        DataManager.instance.LoadGame();

        player.GetComponent<PlayerController>().RespawnPlayer();
    }
}