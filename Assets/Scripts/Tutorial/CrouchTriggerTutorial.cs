using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchTriggerTutorial : MonoBehaviour
{
    [SerializeField] private InputActionReference crouchInput;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.GetInstance().GetPlayerHUD().SetTutorialCrouchText(crouchInput);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.GetInstance().GetPlayerHUD().HideInteract();
    }
}
