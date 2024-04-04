using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class VisionEffect : MonoBehaviour {
    public Color targetColor = Color.black;
    Color currColor = Color.black;

    [Range(0f, 1f)]
    public float targetStrength = 1f;
    float currStrength;

    [Range(1f, 200f)]
    public float targetIntensity = 40f;
    float currIntensity;

    [Range(0f, .1f)]
    public float transitionSpeed = 0.02f;

    public Material material;

    // Start is called before the first frame update
    void Start() {
        material = Instantiate(material);
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().material = material;
        currStrength = material.GetFloat("_Strength");
        currIntensity = material.GetFloat("_Intensity");
        currColor = material.GetColor("_Color");
    }

    // Update is called once per frame
    void Update() {
        // only does updates if the transition speed is nonzero
        if (transitionSpeed > 0f) {
           
            // if there's a difference in strength, ease into target strength value
            if (targetStrength < currStrength) {
                currStrength = Mathf.Floor((currStrength - (currStrength - targetStrength) * transitionSpeed) * 100f) / 100f;
                material.SetFloat("_Strength", currStrength);
            }
            else if (targetStrength > currStrength) {
                currStrength = Mathf.Ceil((currStrength + (targetStrength - currStrength) * transitionSpeed) * 100f) / 100f;
                material.SetFloat("_Strength", currStrength);
            }

            // if there's a difference in intensity, ease into target intensity value
            if (targetIntensity < currIntensity) {
                currIntensity = Mathf.Floor(currIntensity - (currIntensity - targetIntensity) * transitionSpeed);
                material.SetFloat("_Intensity", currIntensity);
            }
            else if (targetIntensity > currIntensity) {
                currIntensity = Mathf.Ceil(currIntensity + (targetIntensity - currIntensity) * transitionSpeed);
                material.SetFloat("_Intensity", currIntensity);
            }

            // if any of the RGB channels of the current color are diff from the target color
            if (currColor.r != targetColor.r || currColor.g != targetColor.g || currColor.b != targetColor.g) {
                // for each color channel, ease into the target color if there's a difference
                if (currColor.r < targetColor.r) {
                    currColor.r = Mathf.Floor((currColor.r - (currColor.r - targetColor.r) * transitionSpeed) * 1000f) / 1000f;
                }
                else if (currColor.r > targetColor.r) {
                    currColor.r = Mathf.Ceil((currColor.r + (targetColor.r - currColor.r) * transitionSpeed) * 1000f) / 1000f;
                }

                if (currColor.g < targetColor.g) {
                    currColor.g = Mathf.Floor((currColor.g - (currColor.g - targetColor.g) * transitionSpeed) * 1000f) / 1000f;
                }
                else if (currColor.g > targetColor.g) {
                    currColor.g = Mathf.Ceil((currColor.g + (targetColor.g - currColor.g) * transitionSpeed) * 1000f) / 1000f;
                }

                if (currColor.b < targetColor.b) {
                    currColor.b = Mathf.Floor((currColor.b - (currColor.b - targetColor.b) * transitionSpeed) * 1000f) / 1000f;
                }
                else if (currColor.b > targetColor.b) {
                    currColor.b = Mathf.Ceil((currColor.b + (targetColor.b - currColor.b) * transitionSpeed) * 1000f) / 1000f;
                }

                material.SetColor("_Color", currColor);
            }
        }
    }
}
