using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected EnemyController eController;
    public void SetReference(EnemyController controller)
    {
        if(eController == null)
            eController = controller;
    }
    public abstract void InitializeState();
    public abstract State RunCurrentState();
}