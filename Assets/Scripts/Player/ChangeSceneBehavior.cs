using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
using UnityEngine.SceneManagement;

public class ChangeSceneBehavior : MonoBehaviour
{
    [SerializeField] string GoToScene;
    [SerializeField] string SpawnPointName;

    public ScreenTransition TransitionComponent = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {

        Debug.Log($"Scene trigger hit, found: {other.gameObject.name}");

        if (other.gameObject.tag == "Player") {
            if (GoToScene != null && SpawnPointName != null) {
                if (TransitionComponent != null) {

                    TransitionComponent.Out(() => {
                        TransitionScene(other.gameObject.GetComponent<PlayerController>());
                    });
                }
                else {
                    TransitionScene(other.gameObject.GetComponent<PlayerController>());
                }
            } else {
                Debug.Log("Please input Scene name and Spawn name");
            }
        }
    }

    void TransitionScene(PlayerController controller) {

        

        DataManager.instance.Data.CurrentScene = GoToScene;

        // put the spawn point name into the data manager script so the player controller can use it
        DataManager.instance.Data.SpawnPointName = SpawnPointName;

        Debug.Log($"Player controller: {controller}");

        // if there is an object held, bring it to the next scene
        if (controller.heldObject != null) {
            // save the object being held
            GameObject obj = controller.heldObject;
            obj.transform.parent = null;

            // makes it so the object is not destroyed when going between scenes
            DontDestroyOnLoad(obj);

            // pass the held object into the data manager so it can be taken into the new scene
            DataManager.instance.Data.heldObj = obj;
        }

        DataManager.instance.SaveGame();

        // load the scene
        SceneManager.LoadScene(GoToScene);

        //Debug.Log($"Data: held obj {DataManager.heldObj.name}");

    }
}
