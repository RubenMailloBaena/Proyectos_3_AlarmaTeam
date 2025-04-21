using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCommand : ICheatCommand
{
    public string Name => "c";
    public void Execute(string[] args)
    {
        GameManager.GetInstance().SetCheckpoint();
    }
}
