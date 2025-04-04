using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeenState : State
{
    public override void InitializeState()
    {
        throw new System.NotImplementedException();
    }

    public override State RunCurrentState()
    {
        print("SEEN LOGIC");
        return this;
    }
}
