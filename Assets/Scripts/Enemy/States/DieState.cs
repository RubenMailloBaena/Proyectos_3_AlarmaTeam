using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : State
{
    public override void InitializeState()
    {
        eController.EnemyDead();
        eController.StopAgent();
        eController.SetRenderActive(false);
    }

    public override State RunCurrentState()
    {
        return this;
    }
}
