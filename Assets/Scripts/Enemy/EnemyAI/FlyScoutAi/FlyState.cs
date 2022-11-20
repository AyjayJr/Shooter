using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiFlyStateId
{
    Idle,
    Chase,
    Death,
    Attack
}

public interface FlyState
{
    AiFlyStateId GetId();
    void Enter(ScoutDroidController agent);
    void Update(ScoutDroidController agent);
    void Exit(ScoutDroidController agent);

}