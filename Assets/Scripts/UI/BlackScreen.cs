using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackScreen : MonoBehaviour {

    public ScreenTransition TransitionComponent;
    [HideInInspector] public TextMeshProUGUI TitleText;
    [HideInInspector] public TextMeshProUGUI SubText;
    [HideInInspector] public TextMeshProUGUI ExitText;

    bool fadeTitle = false;
    bool fadeSub = false;
    bool fadeExit = false;
    bool fadeIn = true;

    bool fadeDone = true;

    public float FadeLength = 1.0f;

    float elapsedTime = 0f;

    System.Action onComplete = () => { return; };

    // Start is called before the first frame update
    void Start() {
        TitleText = this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        TitleText.color = new Color(1, 1, 1, 0);
        SubText = this.gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        SubText.color = new Color(1, 1, 1, 0);
        ExitText = this.gameObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        ExitText.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update() {
        if (!fadeDone) {
            elapsedTime += Time.fixedDeltaTime;

            float progress = (fadeIn ? elapsedTime / FadeLength : 1 - (elapsedTime / FadeLength));
            if (fadeTitle) TitleText.color = new Color(1, 1, 1, progress);
            if (fadeSub) SubText.color = new Color(1, 1, 1, progress);
            if (fadeExit) ExitText.color = new Color(1, 1, 1, progress);

            if (elapsedTime >= FadeLength) {
                fadeTitle = false; fadeSub = false; fadeExit = false; fadeDone = true;
                onComplete();
            }
        }
    }

    public void Enter() {
        resetTransComp();
        TransitionComponent.Out(() => {
            Fade(true, false, false, true, () => {
                Fade(false, true, false, true, () => {
                    Fade(false, false, true, true, () => { return; });
                });
            });
        });
    }

    public void Enter(System.Action complete) {
        resetTransComp();
        TransitionComponent.Out(() => {
            Fade(true, false, false, true, () => {
                Fade(false, true, false, true, () => {
                    Fade(false, false, true, true, complete);
                });
            });
        });
    }

    public void Exit(System.Action complete) {
        Fade(true, true, true, false, () => {
            resetTransComp();
            TransitionComponent.In(complete);
        });
    }

    public void Exit() {
        Fade(true, true, true, false, () => {
            resetTransComp();
            TransitionComponent.In(() => {});
        });
    }

    public void ExitTextOnly() {
        Fade(true, true, true, false, () => {});
    }

    public void ExitTextOnly(System.Action complete) {
        Fade(true, true, true, false, complete);
    }

    public void ExitScreenOnly() {
        resetTransComp();
        TransitionComponent.Out(()=> { });
    }

    public void ExitScreenOnly(System.Action complete) {
        resetTransComp();
        TransitionComponent.Out(complete);
    }

    void Fade(bool title, bool sub, bool exit, bool fadein, System.Action complete) {
        fadeTitle = title;
        fadeSub = sub;
        fadeExit = exit;
        fadeIn = fadein;

        onComplete = complete;

        elapsedTime = 0f;
        fadeDone = false;
    }

    void resetTransComp() {
        TransitionComponent.color = Color.black;
        TransitionComponent.FadeOutStyle = ScreenTransition.style.Fade;
        TransitionComponent.FadeOutDuration = 2.0f;
    }
}
