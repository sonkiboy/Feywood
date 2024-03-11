using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ChangeSceneBehavior : MonoBehaviour
{
    [SerializeField] string GoToScene;
    [SerializeField] string SpawnPointName;



    bool isTransitioning = false;
    

    IEnumerator TransitionScene(PlayerController controller)
    {
        if(GoToScene != null && SpawnPointName != null)
        {
            Debug.Log($"Attemping to go to scene {GoToScene} at {SpawnPointName} with object: {controller.heldObject.name}");

            // play transision


            // put the spawn point name into the data manager script so the player controller can use it
            DataManager.SpawnName = SpawnPointName;

            // if there is an object held, bring it to the next scene
            if(controller.heldObject != null)
            {
                // save the object being held
                GameObject obj = controller.heldObject;
                obj.transform.parent = null;

                // makes it so the object is not destroyed when going between scenes
                DontDestroyOnLoad(obj);

                // pass the held object into the data manager so it can be taken into the new scene
                DataManager.heldObj = obj;
            }
            

            // load the scene
            SceneManager.LoadScene(GoToScene);

            Debug.Log($"Data: held obj {DataManager.heldObj.name}");

        }
        else
        {
            Debug.Log("Please input Scene name and Spawn name");
            yield break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(TransitionScene(other.gameObject.GetComponent<PlayerController>()));
        }
    }
}
