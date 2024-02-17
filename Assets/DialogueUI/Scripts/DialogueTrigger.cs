using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DialogueUI
{
    [System.Serializable]
    public class DialogueCharacter
    {
        public string name;
        public Sprite icon;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public DialogueCharacter character;
        [TextArea(3,10)]
        public string line;
    }

    [System.Serializable]
    public class Dialogue
    {
        public List<DialogueLine> dialogueLines = new List<DialogueLine>(); 
    }
    public class DialogueTrigger : MonoBehaviour
    {
        public Dialogue dialogue;

        public FeywoodPlayerActions playerControls;
        private InputAction interact;
        private bool withinInteractRange;

        public void TriggerDialogue()
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }

        private void Awake()
        {
            playerControls = new FeywoodPlayerActions();
            GetComponentInChildren<CanvasGroup>().alpha = 0;
        }

        private void OnEnable()
        {
            interact = playerControls.Player.Interact;
            interact.Enable();
            interact.performed += Interact;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                withinInteractRange = true;
                GetComponentInChildren<CanvasGroup>().alpha = 1;
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if(collision.tag == "Player")
            {
                withinInteractRange = false;
                GetComponentInChildren<CanvasGroup>().alpha = 0;
            }
        }

        void Interact(InputAction.CallbackContext context)
        {
            if(withinInteractRange)
            {
                TriggerDialogue();
            }
        }
    }
}