using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCommand : ICheatCommand
{
    public string Name => "exit";
    public void Execute(string[] args)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LevelChangeManager.GetInstance().GoToMainMenu();
    }
}
