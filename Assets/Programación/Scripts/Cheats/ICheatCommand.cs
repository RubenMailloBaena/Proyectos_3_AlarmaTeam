using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICheatCommand
{
    string Name { get; }
    void Execute(string[] args);
}
