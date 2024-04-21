using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    //stealing the playercontroller script
    PlayerController _playerController;
    //create a collider that starts cutscene
    [SerializeField] Collider cutsceneCollider;
    //bool that determines if dialogue is occuring
    public bool Dialogue;
    public bool cutsceneTriggered = false;
    //bool to determine if player can move
    public bool canMove;
    // Start is called before the first frame update
    void Start()
    {
        _playerController= GetComponent<PlayerController>();
    }
    //Update is called once per frame
    void Update()
    {
        //checks for player collision with cutscene start
        if (cutsceneCollider.gameObject.CompareTag("Player")&& !cutsceneTriggered)
        {
            StartCoroutine(CutScene());
        }
        else
        {
            StopCoroutine(CutScene());
        }
    }
    IEnumerator CutScene()
    {
        cutsceneTriggered = true;
        Dialogue = true;

        while (Dialogue)
        {
            canMove = false;

            //if (condition)
            //{
            //    DialogueEnd();
            //}
        }
        yield return null;
    }
    public void DialogueEnd()
    {
        Dialogue= false;
    }

}
