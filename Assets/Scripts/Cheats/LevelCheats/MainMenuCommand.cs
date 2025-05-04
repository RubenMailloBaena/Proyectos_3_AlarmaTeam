using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCommand : ICheatCommand
{
    public string Name => "exit";
    public void Execute(string[] args)
    {
        LevelChangeManager.GetInstance().GoToMainMenu();
    }
}
