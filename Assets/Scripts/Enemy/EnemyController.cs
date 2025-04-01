using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private State currentState, nextState;

    [Header("INITIAL STATE")]
    [SerializeField] private IdleState idleState;

    public List<State> allStates;

    void Start()
    {
        SwitchToNextState(idleState);
    }

    void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        nextState = currentState?.RunCurrentState();

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
