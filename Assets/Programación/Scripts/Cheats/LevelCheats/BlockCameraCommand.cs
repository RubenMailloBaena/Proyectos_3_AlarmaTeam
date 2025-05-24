using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCameraCommand : ICheatCommand
{
    public string Name => "lcam";

    public void Execute(string[] args)
    {
        GameManager.GetInstance().GetPlayerController().LockCamera(true);
    }
}
