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

    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
    }

    private void Update()
    {
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
        string bindingPath = input.bindings[1].effectivePath;

        string displayString = InputControlPath.ToHumanReadableString(bindingPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        UIText.text = displayString;
    }    

    private void OnEnable()
    {
        GameManager.GetInstance().GetPlayerController().OnCanInteract += SetInteractionText;
    }

    private void OnDisable()
    {
        GameManager.GetInstance().GetPlayerController().OnCanInteract -= SetInteractionText;
    }
    
    /*private void CanInteractText()
    {

        foreach (InputBinding binding in testInput.action.bindings)
        {
            string bindingPath = binding.effectivePath;

            string displayString = InputControlPath.ToHumanReadableString(bindingPath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);

            Debug.Log("Binding: " + displayString);
            UIText.text = displayString;
        }
    }*/
}
