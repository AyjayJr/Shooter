using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderStateMachine
{
    public ExploderState[] states;
    public ExploderController agent;
    public ExploderStateId currentState;


    public ExploderStateMachine(ExploderController agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(AiStateId)).Length;
        states = new ExploderState[numStates];
    }

    public void RegisterState(ExploderState state)
    {
        int index = (int)state.GetId();
        states[index] = state;

    }

    public ExploderState GetState(ExploderStateId stateId) 
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(ExploderStateId newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
