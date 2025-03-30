using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private State currentState;

    [Header("INITIAL STATE")]
    [SerializeField] private IdleState idleState;

    void Start()
    {
        SwitchToNextState(idleState);
    }

    void FixedUpdate()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();

        if (nextState != null && nextState != currentState)
            SwitchToNextState(nextState);
    }

    private void SwitchToNextState(State nextState)
    {
        print("Entering: " + nextState.name);
        currentState = nextState;
        currentState?.InitializeState();
    }
}
