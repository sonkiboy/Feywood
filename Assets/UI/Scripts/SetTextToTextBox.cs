using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.XInput;

[RequireComponent(typeof(TMP_Text))]
public class SetTextToTextBox : MonoBehaviour
{
    [TextArea(2, 3)]
    [SerializeField] private string message = "BUTTONPROMPT";

    [Header("Setup for sprites")]
    [SerializeField] private ListofTmpAssets listOfTmpSpriteAssets;
    [SerializeField] private DeviceType deviceType;

    private FeywoodPlayerActions _playerInput;
    private PlayerInput _playerInput2;
    private TMP_Text _textBox;
    private void Awake()
    {
        _playerInput = new FeywoodPlayerActions();
        _playerInput2 = FindObjectOfType<PlayerInput>();
        _textBox = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        SetText();
    }

    private void Update()
    {
        //Debug.Log(_playerInput2.currentControlScheme);
    }
    void OnEnable()
    {
        InputUser.onChange += onInputDeviceChange;
    }

    void OnDisable()
    {
        InputUser.onChange -= onInputDeviceChange;
    }

    void onInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged)
        {
            Debug.Log(user.controlScheme.Value.name);
            if (user.controlScheme.Value.name.Equals("Keyboard&Mouse"))
            {
                //Debug.Log("Device Changed");
                deviceType = DeviceType.Keyboard;
                //Debug.Log((int)deviceType);
            }
            else if (user.controlScheme.Value.name.Equals("Xbox"))
            {
                //Debug.Log("Device Changed");
                deviceType = DeviceType.XboxController;
                //Debug.Log((int)deviceType);
            }
            SetText();
        }
    }

    [ContextMenu(itemName: "Set Text")]

    private void SetText()
    {

        if ((int)deviceType > listOfTmpSpriteAssets.SpriteAssets.Count - 1)
        {
            Debug.Log($"Missing Sprite Asset for {deviceType}");
            return;
        }

        _textBox.text = CompleteTextWithButtonPromptSprite.ReadAndReplaceBinding(
            message,
            _playerInput.Player.Interact.bindings[(int)deviceType],
            listOfTmpSpriteAssets.SpriteAssets[(int)deviceType]
            );
    }

    private enum DeviceType
    {
        XboxController = 0,
        Keyboard = 1
    }
}

