using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public BlackScreen BlackScreenTextComponent;
    public PlayerController playerController;

    public string TitleText = "You failed...";
    [TextArea] public string HintText;
    // Start is called before the first frame update

    [Tooltip("Leave blank if not switching scenes.")]
    [SerializeField] string GoToScene;
    [Tooltip("Leave blank if not switching scenes.")]
    [SerializeField] string SpawnPointName;

    void Start()
    {

    }

    //private void OnTriggerEnter(Collider other) {
    //    if (other.gameObject.tag == "Player") {
    //        if (!string.IsNullOrWhiteSpace(TitleText)) BlackScreenTextComponent.TitleText.text = TitleText;
    //        BlackScreenTextComponent.SubText.text = "Tip:\n" + HintText;

    //        playerController.currentRestriction = PlayerController.MovementRestrictions.noMovement; // this doesn't seem to be working
    //        BlackScreenTextComponent.Enter();
    //    }
    //}
    public void Trigger()
    {
        if (!string.IsNullOrWhiteSpace(TitleText)) BlackScreenTextComponent.TitleText.text = TitleText;
        BlackScreenTextComponent.SubText.text = "Tip:\n" + HintText;

        playerController.currentRestriction = PlayerController.MovementRestrictions.noMovement; // this doesn't seem to be working
        BlackScreenTextComponent.Enter();
    }

    public void Exit()
    { // i don't know where to put this but this is what to when you want to exit the screen; should happen after generic key/buttonpress
        playerController.currentRestriction = PlayerController.MovementRestrictions.None;
        // place anything you want to happen instantaneously here
        BlackScreenTextComponent.ExitTextOnly(() =>
        {
            // place anything you want to happen once the text itself disappears, but the screen is still black, here

            // if scene is specified, move to that scene

            // if both scene to go to and spawn point are specified, specify spawnpoint before moving to scene

            // if only spawn point is specified, reset player to that location (no scene move)

            BlackScreenTextComponent.ExitScreenOnly(); // only relevant to location reset; if you move out of the scene you'll never see this. fades out of black screen
        });
    }
}
