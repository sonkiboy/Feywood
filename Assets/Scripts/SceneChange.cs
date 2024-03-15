using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public int TargetSceneIndex = 3;
    public GameObject TransitionComponent = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            if (TransitionComponent != null) {
                ScreenTransition transition = TransitionComponent.GetComponent<ScreenTransition>();

                transition.Out(() => {
                    SceneManager.LoadScene(TargetSceneIndex);
                });
            } else {
                SceneManager.LoadScene(TargetSceneIndex);
            }
        }
    }
}
