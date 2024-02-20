using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SneakUI : MonoBehaviour
{

    Image uiImage;

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
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        uiImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
