using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleCommand : ICheatCommand
{
    public string Name => "ts";
    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Debug.Log("No arguments, reseted TimeScale");
            Time.timeScale = 1.0f;
            return;
        }

        if (float.TryParse(args[0], out float newTimeScale))
        {
            Time.timeScale = newTimeScale;
        }
    }
}
