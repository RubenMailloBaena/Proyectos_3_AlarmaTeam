using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheatUI : MonoBehaviour
{
    [Header("UI Cheat Elements")]
    public GameObject cheatPanel;
    public TMP_InputField inputField;

    private bool isOpen = false;
    private Dictionary<string, ICheatCommand> cheatCommands;

    void Awake()
    {
        CloseConsole();
        RegisterCheats();
    }

    void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand(inputField.text);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isOpen)
                CloseConsole();
            else
                OpenConsole();
        }
    }

    public void OpenConsole()
    {
        isOpen = true;
        cheatPanel.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
        Time.timeScale = 0f;
    }

    public void CloseConsole()
    {
        isOpen = false;
        cheatPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void AddCommand(ICheatCommand command)
    {
        cheatCommands[command.Name.ToLower()] = command;

    }

    private void ExecuteCommand(String name)
    {
        string[] split = name.Trim().Split(' '); //Separamos los espacios. Ejemplo: "vida 100" -> ["vida","100"]
        string commandName = split[0].ToLower(); // Pasamos el primer elemento que siempre es el nombre del command a minsuculas para evitar problemas. Obtenemos "vida". 
        string[] args = new string[split.Length - 1]; // Creamos un Array para almacenar los siguientes valores. Obtenemos "100". 

        for (int i = 1; i < split.Length; i++)
        {
            args[i - 1] = split[i];
        }
        // Recorremos el array separado para guardar todos los valores que no sean el nombre. En este caso sera 1 y guardaremos el "100".

        if (cheatCommands.ContainsKey(commandName))
        {
            cheatCommands[commandName].Execute(args);
            Debug.Log("Command Executed: " + commandName);
        }
        else
            Debug.LogWarning("Command does not exist: " + commandName);

        
        inputField.ActivateInputField(); // Reactivamos el field para evitar hacer clics manuales constantemente. 

    }
    
    private void RegisterCheats()
    {
        cheatCommands = new Dictionary<string, ICheatCommand>();
        AddCommand(new RestartCommand());
        AddCommand(new BlockCameraCommand());
        AddCommand(new UnblockCameraCommand());
    }
}
