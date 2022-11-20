using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyStateMachine
{
    public FlyState[] states;
    public ScoutDroidController agent;
    public AiFlyStateId currentState;


    public FlyStateMachine(ScoutDroidController agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(AiStateId)).Length;
        states = new FlyState[numStates];
    }

    public void RegisterState(FlyState state)
    {
        int index = (int)state.GetId();
        states[index] = state;

    }

    public FlyState GetState(AiFlyStateId stateId) 
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(AiFlyStateId newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
