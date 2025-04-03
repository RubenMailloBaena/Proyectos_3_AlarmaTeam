using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckState : State
{
    public override void InitializeState()
    {
        eController.soundWasAnObject = true; //RESTART PLAYER HEAR
        eController.StopAgent();
    }

    public override State RunCurrentState()
    {
        return this;
    }
}
