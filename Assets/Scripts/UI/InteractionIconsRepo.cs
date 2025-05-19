using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionIconsRepo : MonoBehaviour
{
    [Header("Input Types")] 
    [SerializeField] private string pressString;
    [SerializeField] private string holdString;
    
    [Header("Action Types")] 
    [SerializeField] private string defaultString;
    [SerializeField] private string backstabString;
    [SerializeField] private string teleportString;
    [SerializeField] private string vaultString;
    [SerializeField] private string crouchString;
    
    [Space(20)]
    [SerializeField] private List<KeySpritePair> keySprites;
    
    private Dictionary<string, KeySpritePair> interactionIcons= new Dictionary<string, KeySpritePair>();
    
    private void Awake()
    {
        foreach (KeySpritePair keyIcon in keySprites)
            if(!interactionIcons.ContainsKey(keyIcon.keyName))
                interactionIcons.Add(keyIcon.keyName.ToLower(), keyIcon);
    }

    public KeySpritePair GetInputText(InputAction input, InputType inputType, ActionType action, PlayerInput playerInput)
    {
        if (interactionIcons.TryGetValue(GetKeyOrButtonName(input, playerInput), out KeySpritePair result))
        {
            result.inputTypeText = GetInputTypeString(inputType);
            result.actionTypeText = GetActionTypeString(action);
            return result;
        }
        return null;
    }
    
    private string GetKeyOrButtonName(InputAction input, PlayerInput playerInput)
    {
        print("Here");
        string currentControl = playerInput.currentControlScheme;
        string bindingPath = "";
        
        if(currentControl.Equals("Keyboard&Mouse"))
            bindingPath = input.bindings[0].effectivePath;
        else
            bindingPath = input.bindings[1].effectivePath;

        return InputControlPath.ToHumanReadableString(bindingPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice).ToLower();
    }

    public string GetInputTypeString(InputType input)
    {
        switch (input)
        {
            case InputType.Hold: return holdString;
            default: return pressString;
        }
    }

    public string GetActionTypeString(ActionType action)
    {
        switch (action)
        {
            case ActionType.Backstab: return backstabString;
            case ActionType.Teleport: return teleportString;
            case ActionType.Vault: return vaultString;
            case ActionType.Crouch: return crouchString;
            default: return defaultString;
        }
    }

    public Sprite GetCorrespondingSprite(string key)
    {
        interactionIcons.TryGetValue(key, out KeySpritePair result);
        return result.icon;
    }

}

[Serializable]
public class KeySpritePair
{
    public string keyName; //Nombre de la tecla en Unity (left click)
    public Sprite icon;
    [HideInInspector] public string inputTypeText;
    [HideInInspector] public string actionTypeText;
}
