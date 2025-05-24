using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVaseCommand : MonoBehaviour, ICheatCommand
{
    public string Name => "rvase";

    public void Execute(string[] args)
    {
        // ThrowableObject[] throwableObjects = GameObject.FindObjectsOfType<ThrowableObject>(true);
        //
        // int resetCount = 0;
        // foreach (ThrowableObject obj in throwableObjects)
        // {
        //     obj.ResetObject();
        //     resetCount++;
        // }
        //
        // Debug.Log($"Reset {resetCount} throwable objects.");
    }
}
