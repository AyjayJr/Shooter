using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine
{
    public BossState[] states;
    public BossController agent;
    public BossStateID currentState;


    public BossStateMachine(BossController agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(BossStateID)).Length;
        states = new BossState[numStates];
    }

    public void RegisterState(BossState state)
    {
        int index = (int)state.GetId();
        states[index] = state;

    }

    public BossState GetState(BossStateID stateId) 
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(BossStateID newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
