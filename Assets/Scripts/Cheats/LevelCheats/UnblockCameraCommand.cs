using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnblockCameraCommand : ICheatCommand
{
    public string Name => "ucam";

    public void Execute(string[] args)
    {
        Debug.Log("Unlocked Cursor!");
        GameManager.GetInstance().GetPlayerLookController().UnlockCursor();
    }
}
