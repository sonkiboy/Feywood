using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    BlackScreen BlackScreenTextComponent;


    public string TitleText = "You failed...";
    [TextArea] public string HintText;
    // Start is called before the first frame update

    [Tooltip("Leave blank if not switching scenes.")]
    [SerializeField] string GoToScene;
    [Tooltip("Leave blank if not switching scenes.")]
    [SerializeField] string SpawnPointName;

    void Start()
    {
        BlackScreenTextComponent = FindAnyObjectByType<BlackScreen>();
    }

    //private void OnTriggerEnter(Collider other) {
    //    if (other.gameObject.tag == "Player") {
    //        if (!string.IsNullOrWhiteSpace(TitleText)) BlackScreenTextComponent.TitleText.text = TitleText;
    //        BlackScreenTextComponent.SubText.text = "Tip:\n" + HintText;

    //        playerController.currentRestriction = PlayerController.MovementRestrictions.noMovement; // this doesn't seem to be working
    //        BlackScreenTextComponent.Enter();
    //    }
    //}
    public IEnumerator PlayHint(float duration)
    {
        if (!string.IsNullOrWhiteSpace(TitleText)) BlackScreenTextComponent.TitleText.text = TitleText;
        BlackScreenTextComponent.SubText.text = "Tip:\n" + HintText;

        
        BlackScreenTextComponent.Enter();

        yield return new WaitForSeconds(duration);

        BlackScreenTextComponent.Exit();

    }


    public void StopHint()
    {

    }

    // H: I don't think we need this method, trying to keep it in the Data Manager script
    public void Exit()
    { // i don't know where to put this but this is what to when you want to exit the screen; should happen after generic key/buttonpress
        
        // place anything you want to happen instantaneously here
        BlackScreenTextComponent.ExitTextOnly(() =>
        {
            // place anything you want to happen once the text itself disappears, but the screen is still black, here

            // if scene is specified, move to that scene
            //if (!string.IsNullOrEmpty(GoToScene)) {
            //    DataManager.instance.Data.CurrentScene = GoToScene;

            //    // if both scene to go to and spawn point are specified, specify spawnpoint before moving to scene
            //    // put the spawn point name into the data manager script so the player controller can use it
            //    if (!string.IsNullOrEmpty(SpawnPointName)) {
            //        DataManager.instance.Data.SpawnPointName = SpawnPointName;
            //    }


            //    // if there is an object held, bring it to the next scene
            //    if (playerController.heldObject != null) {
            //        // save the object being held
            //        GameObject obj = playerController.heldObject;
            //        obj.transform.parent = null;

            //        // makes it so the object is not destroyed when going between scenes
            //        DontDestroyOnLoad(obj);

            //        // pass the held object into the data manager so it can be taken into the new scene
            //        DataManager.instance.Data.heldObj = obj;
            //    }

            //    DataManager.instance.SaveGame();

            //    // load the scene
            //    SceneManager.LoadScene(GoToScene);
            //}


            // if only spawn point is specified, reset player to that location (no scene move)



            //BlackScreenTextComponent.ExitScreenOnly(); // only relevant to location reset; if you move out of the scene you'll never see this. fades out of black screen
        });
    }
}
