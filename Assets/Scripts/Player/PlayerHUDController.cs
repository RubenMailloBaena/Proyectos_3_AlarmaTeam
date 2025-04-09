using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum InputType {
    Press,
    Hold
}

public class PlayerHUDController : MonoBehaviour
{
    private PlayerController pController;
    
    [Header("Player Sound")]
    [SerializeField] private Texture sound1;
    [SerializeField] private Texture sound2;
    [SerializeField] private Texture sound3;
    [SerializeField] private RawImage UIImage;

    [Header("Interactions Text")] 
    [SerializeField] private String messageText;
    [SerializeField] private TextMeshProUGUI UIText;
    [SerializeField] private InputActionReference testInput;
    private PlayerInput playerInput;

    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        playerInput = pController.GetPlayerInput();
    }

    private void Update()
    {
        print(playerInput.currentControlScheme);
        HandleSoundVisuals();
    }

    private void HandleSoundVisuals()
    {
        if (!pController.IsIdle)
        {
            UIImage.enabled = true;
            UIImage.texture = GetSoundTexture();
        }
        else
            UIImage.enabled = false;
    }

    private Texture GetSoundTexture()
    {
        if (pController.IsCrouching) return sound1;
        if (!pController.IsRunning) return sound2;
        return sound3;
    }

    private void SetInteractionText(InputAction input, InputType type)
    {
        string currentControl = playerInput.currentControlScheme;
        string bindingPath = "";
        
        if(currentControl.Equals("Keyboard&Mouse"))
            bindingPath = input.bindings[0].effectivePath;
        else
            bindingPath = input.bindings[1].effectivePath;

        string displayString = InputControlPath.ToHumanReadableString(bindingPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        string result = "";
        result = type == InputType.Press ? "Press '" : "Hold '";
        
        result += displayString + "' to interact";
        UIText.enabled = true;
        UIText.text = result;
    }
    
    private void OnEnable()
    {
        GameManager.GetInstance().GetPlayerController().OnCanInteract += SetInteractionText;
    }

    private void OnDisable()
    {
        GameManager.GetInstance().GetPlayerController().OnCanInteract -= SetInteractionText;
    }
}
