using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartFromCheckpointCommand : ICheatCommand
{
    public string Name => "rc";
    public void Execute(string[] args)
    {
        GameManager.GetInstance().RestartFromCheckpoint();
    }
}
