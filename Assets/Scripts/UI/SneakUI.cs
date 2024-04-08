using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SneakUI : MonoBehaviour
{

    Image uiImage;
    VisionEffect vingette;

    [SerializeField] Color HiddenVingetteColor;
    [SerializeField] Color CaughtVingetteColor;

    [SerializeField] float VingetteIntesity;

    [SerializeField] Sprite[] sprites;

    public enum SneakStates
    {
        Exposed,
        Hidden,
        Spotted
    }

    SneakStates currentState = SneakStates.Exposed;

    public SneakStates CurrentState
    {
        get { return currentState; }

        set 
        { 
            currentState = value;

            uiImage.sprite = sprites[(int)value];

            switch (currentState)
            {
                case (SneakStates.Exposed):

                    vingette.targetStrength = 0;

                    break;

                case (SneakStates.Hidden):

                    vingette.targetStrength = 1;

                    vingette.targetColor = HiddenVingetteColor;

                    break;

                case (SneakStates.Spotted):

                    vingette.targetStrength = 1;

                    vingette.targetColor = CaughtVingetteColor;

                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        uiImage = GetComponent<Image>();
        vingette = GameObject.FindAnyObjectByType<VisionEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
