using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Burst;


namespace DialogueUI
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;
        public PlayerController playerController;
        public Image characterIcon;
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI dialogueArea;
        public FeywoodPlayerActions playerControls;
        private InputAction submit;


        private Queue<DialogueLine> lines = new Queue<DialogueLine>();

        public bool isDialogueActive = false;

        public bool isRespawnDialogue = false;

        public bool dialogueEvent2 = false;

        public bool dialogueDelegate = false;

        public float typingSpeed = 0.2f;

        private void Start()
        {
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();

            if (Instance == null)
            {
                Instance = this;
            }
            if (!isDialogueActive)
            {
                GetComponent<CanvasGroup>().alpha = 0;
            }

        }
        private void Awake()
        {
            playerControls = new FeywoodPlayerActions();
        }
        private void OnEnable()
        {
            submit = playerControls.UI.Submit;
            submit.Enable();
            submit.performed += Submit;
        }

        public void StartDialogue(Dialogue dialogue)
        {
            isDialogueActive = true;

            playerController.currentRestriction = PlayerController.MovementRestrictions.noMovement;

            GetComponent<CanvasGroup>().alpha = 1;

            lines.Clear();

            foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
            {
                lines.Enqueue(dialogueLine);
            }

            DisplayNextDialogueLine();
        }
        public void DisplayNextDialogueLine()
        {
            if (lines.Count <= 0)
            {
                EndDialogue();
                return;
            }

            DialogueLine currentLine = lines.Dequeue();

            //characterIcon.sprite = currentLine.character.icon;
            characterName.text = currentLine.character.name;

            StopAllCoroutines();

            StartCoroutine(TypeSentence(currentLine));
        }
        IEnumerator TypeSentence(DialogueLine dialogueLine)
        {
            dialogueArea.text = "";
            foreach (char letter in dialogueLine.line.ToCharArray())
            {
                dialogueArea.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        void EndDialogue()
        {
            isDialogueActive = false;

            playerController.currentRestriction = PlayerController.MovementRestrictions.None;
            GetComponent<CanvasGroup>().alpha = 0;

            if (isRespawnDialogue)
            {
                onRespawnDialogueEnd.Invoke();
            }
            else if (dialogueEvent2)
            {
                onDialogueEnd2.Invoke();
            }
            else
            {
                onDialogueEnd.Invoke();
            }

        }

        void Submit(InputAction.CallbackContext context)
        {
            if (isDialogueActive)
            {
                Debug.Log("next line");
                this.DisplayNextDialogueLine();
            }
        }

        public UnityEvent onDialogueEnd;
        public UnityEvent onDialogueEnd2;
        public UnityEvent onRespawnDialogueEnd;
    }
}