using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodModeCommand : ICheatCommand
{
    public string Name => "gm";
    public void Execute(string[] args)
    {
        GameManager.GetInstance().GetPlayerController().SwapGodMode();
    }
}
