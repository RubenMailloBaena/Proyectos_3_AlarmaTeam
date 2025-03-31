using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartCommand : ICheatCommand
{
    public string Name => "rlvl";

    public void Execute(string[] args)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
