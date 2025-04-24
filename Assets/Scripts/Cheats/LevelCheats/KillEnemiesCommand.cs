using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemiesCommand : ICheatCommand
{
    public string Name => "k";

    public void Execute(string[] args)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject enemy in enemies)
        {
            GameObject.Destroy(enemy);
        }
    }

   
}
