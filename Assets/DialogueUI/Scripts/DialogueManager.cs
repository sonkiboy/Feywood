using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;


namespace DialogueUI
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;
        public PlayerController playerController;
        public Image characterIcon;
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI dialogueArea;

        private Queue<DialogueLine> lines = new Queue<DialogueLine>();

        public bool isDialogueActive = false;

        public float typingSpeed = 0.2f;

        private void Start()
        {
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();


            if(Instance == null)
            {
                Instance = this;
            }
            if(!isDialogueActive )
            {
                GetComponent<CanvasGroup>().alpha = 0;
            }

        }
        public void StartDialogue(Dialogue dialogue)
        {
            isDialogueActive=true;
            
            playerController.currentRestriction = PlayerController.MovementRestrictions.noMovement;

            GetComponent<CanvasGroup>().alpha = 1;

            lines.Clear();

            foreach(DialogueLine dialogueLine in dialogue.dialogueLines)
            {
                lines.Enqueue(dialogueLine);
            }

            DisplayNextDialogueLine();
        }
        public void DisplayNextDialogueLine()
        {
            if(lines.Count <= 0)
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

            onDialogueEnd.Invoke();
        }

        public UnityEvent onDialogueEnd;
    }
}