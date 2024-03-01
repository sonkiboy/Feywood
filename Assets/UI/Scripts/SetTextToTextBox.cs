using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
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
    private TMP_Text _textBox;
    private void Awake()
    {
        _playerInput = new FeywoodPlayerActions();
        _textBox = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        SetText();
    }

    private void Update()
    {
        InputSystem.onDeviceChange +=
        (device, change) =>
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log($"Device {device} was added");
                break;
            case InputDeviceChange.Removed:
                Debug.Log($"Device {device} was removed");
                break;
        }
    };
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

