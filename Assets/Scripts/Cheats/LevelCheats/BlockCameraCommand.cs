using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCameraCommand : ICheatCommand
{
    public string Name => "lcam";

    public void Execute(string[] args)
    {
        Debug.Log("Locked Cursor!");
        GameManager.GetInstance().GetPlayerLookController().LockCursor();
    }
}
