// In this example we show how to invoke a coroutine and wait until it
// is completed

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour {

    bool playing = true;
    bool reversed = true;
    float duration = 10f;
    float elapsedTime = 0f;
    public Material material;
    System.Action onComplete = () => { return; };

    public float Smoothing = 0.3f;

    public enum style {
        Vignette, Up_Down, Down_Up,
        Left_Right, Right_Left,
        Fade, Image, Image_Invert
    }

    public Color color = Color.black;
    public Texture2D TransitionImg;

    public float FadeInDuration = 2f;
    public style FadeInStyle = style.Fade;

    public float FadeOutDuration = 2f;
    public style FadeOutStyle = style.Fade;

    void Start() {
        material = Instantiate(material);
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().material = material;
        In(onComplete);
    }

    void Update() {
        if (playing) {
            elapsedTime += Time.fixedDeltaTime;
            float progress = elapsedTime / duration;

            material.SetFloat("_Position", (reversed ? 1f - progress : progress));

            if (elapsedTime >= duration) {
                playing = false;
                reversed ^= true;
                onComplete();
                onComplete = () => { return; }; // reset oncomplete function
            }
        }
    }

    public void In() {
        elapsedTime = 0f;
        material.SetColor("_Color", color);
        if (TransitionImg != null) material.SetTexture("_TransImg", TransitionImg);
        duration = FadeInDuration;
        setStyle(FadeInStyle);
        reversed = true;

        playing = true;
    }

    public void In(System.Action complete) {
        onComplete = complete;
        In();

        playing = true;
    }

    public void Out() {
        elapsedTime = 0f;
        material.SetColor("_Color", color);
        if (TransitionImg != null) material.SetTexture("_TransImg", TransitionImg);
        duration = FadeOutDuration;
        setStyle(FadeOutStyle);
        reversed = false;

        playing = true;
    }
    public void Out(System.Action complete) {
        onComplete = complete;
        Out();

        playing = true;
    }

    public bool isDone() {
        return elapsedTime >= duration;
    }

    public void setStyle(style option) {
        material.DisableKeyword("_TYPE_VIGNETTE");
        material.DisableKeyword("_TYPE_UP_DOWN");
        material.DisableKeyword("_TYPE_DOWN_UP");
        material.DisableKeyword("_TYPE_RIGHT_LEFT");
        material.DisableKeyword("_TYPE_LEFT_RIGHT");
        material.DisableKeyword("_TYPE_FADE");
        material.DisableKeyword("_TYPE_IMAGE");
        material.DisableKeyword("_TYPE_IMAGE_INVERT");

        switch (option) {
            case style.Vignette:
                material.EnableKeyword("_TYPE_VIGNETTE");
                break;
            case style.Up_Down:
                material.EnableKeyword("_TYPE_UP_DOWN");
                break;
            case style.Down_Up:
                material.EnableKeyword("_TYPE_DOWN_UP");
                break;
            case style.Right_Left:
                material.EnableKeyword("_TYPE_RIGHT_LEFT");
                break;
            case style.Left_Right:
                material.EnableKeyword("_TYPE_LEFT_RIGHT");
                break;
            case style.Fade:
                material.EnableKeyword("_TYPE_FADE");
                break;
            case  style.Image:
                material.EnableKeyword("_TYPE_IMAEG");
                break;
            case style.Image_Invert:
                material.EnableKeyword("_TYPE_IMAGE_INVERT");
                break;
        }
    }
}
