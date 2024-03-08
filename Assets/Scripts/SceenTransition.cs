// In this example we show how to invoke a coroutine and wait until it
// is completed

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour {

    public bool enabled = true;
    public bool reversed = false;
    public float duration = 10f;
    public float elapsedTime = 0f;
    private Material material;
    private System.Action onComplete = () => { return; };

    void Start() {
        material = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().material;
        elapsedTime = 0f;
        material.SetFloat("_Position", (reversed ? 1f : 0f));
    }

    void Update() {
        if (enabled) {
            elapsedTime += Time.fixedDeltaTime;
            float progress = elapsedTime / duration;

            material.SetFloat("_Position", (reversed ? 1f - progress : progress));

            if (elapsedTime >= duration) {
                enabled = false;
                onComplete();
            }
        }
    }

    public void Play(System.Action complete) {
        elapsedTime = 0f;
        enabled = true;
        reversed = false;
        onComplete = complete;
    }

    public void PlayReverse(System.Action complete) {
        elapsedTime = 0f;
        enabled = true;
        reversed = true;
        onComplete = complete;
    }

    public bool isDone() {
        return elapsedTime >= duration;
    }

}
