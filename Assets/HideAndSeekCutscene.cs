using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.XR;
using static MicahW.PointGrass.PointGrassCommon;

public class HideAndSeekCutscene : MonoBehaviour
{
    CinemachineVirtualCamera cutsceneCamera;
    public Transform grabPosition;
    private Transform startPosition;

    bool startSequence = false;
    bool endSequence = false;
    GameObject sister;
    GameObject hand;
    PlayerController controller;

    public float speed = 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = FindFirstObjectByType<PlayerController>();
        cutsceneCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        hand = GameObject.Find("Hand");
    }

    // Update is called once per frame
    void Update()
    {
        if (startSequence)
        {
            Debug.Log("Moving Hand");
            hand.transform.position = Vector3.MoveTowards(hand.transform.position, grabPosition.position, speed * Time.deltaTime);
        }
        if((hand.transform.position - grabPosition.position).magnitude < 1f && !endSequence)
        {
            endSequence = true;
            Debug.Log("Demo Over");
            startSequence = false;
            StartCoroutine(DataManager.instance.HintLoad(3f, GetComponent<GameOverScreen>()));
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            startCutscene();
        }
    }

    private void startCutscene()
    {
        controller.currentRestriction = PlayerController.MovementRestrictions.noMovement;
        sister = GameObject.Find("Sister(Clone)");
        cutsceneCamera.Follow = sister.transform;
        cutsceneCamera.LookAt = sister.transform;
        startSequence = true;
    }
}
