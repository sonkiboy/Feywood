using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI 
{
    public static class CompleteTextWithButtonPromptSprite
    {
        public static string ReadAndReplaceBinding(string textToDisplay, InputBinding actionNeeded, TMP_SpriteAsset spriteAsset)
        {
            string stringButtonName = actionNeeded.ToString();
            stringButtonName = RenameInput(stringButtonName);

            textToDisplay = textToDisplay.Replace(
                "BUTTONPROMPT",
                $"<sprite=\"{spriteAsset.name}\" name=\"{stringButtonName}\">");

            return textToDisplay;
        }

        public static string RenameInput(string stringButtonName)
        {
            stringButtonName = stringButtonName.Replace(
                "Interact:",
                string.Empty);

            stringButtonName = stringButtonName.Replace(
                "<Keyboard>/",
                "Keyboard_");

            stringButtonName = stringButtonName.Replace(
                "<XInputController>/",
                "XInputController_");

            return stringButtonName;
        }
    }
}
