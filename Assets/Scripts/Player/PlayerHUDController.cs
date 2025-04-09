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

    [Header("Teleport Bar")]
    [SerializeField] private RawImage UIProgressBar;

    private void Start()
    {
        pController = GameManager.GetInstance().GetPlayerController();
        playerInput = pController.GetPlayerInput();
        UIProgressBar.transform.localScale = new Vector3(0, UIProgressBar.transform.localScale.y, UIProgressBar.transform.localScale.z);
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

    private void HideInteract()
    {
        UIText.enabled = false;
    }

    private void UpdateProgressBar(float progress)
    {
        float normalizedProgress = Mathf.Clamp01(progress/2f);
        Vector3 currentScale = UIProgressBar.transform.localScale;
        currentScale.x = normalizedProgress;
        UIProgressBar.transform.localScale = currentScale;
    }

    private void HideProgressBar()
    {
        UIProgressBar.enabled = false;
    }
    
    private void OnEnable()
    {
        GameManager.GetInstance().GetPlayerController().OnCanInteract += SetInteractionText;
        GameManager.GetInstance().GetPlayerController().OnHideInteraction += HideInteract;
        GameManager.GetInstance().GetPlayerController().OnProgressBar += UpdateProgressBar;
        GameManager.GetInstance().GetPlayerController().OnHideBar += HideProgressBar;
    }

    private void OnDisable()
    {
        GameManager.GetInstance().GetPlayerController().OnCanInteract -= SetInteractionText;
        GameManager.GetInstance().GetPlayerController().OnHideInteraction -= HideInteract;
        GameManager.GetInstance().GetPlayerController().OnProgressBar -= UpdateProgressBar;
        GameManager.GetInstance().GetPlayerController().OnHideBar -= HideProgressBar;
    }
}
