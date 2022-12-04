using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFlyDeathState : FlyState
{
    public void Enter(ScoutDroidController enemyController)
    {
        enemyController.GetComponent<Rigidbody>().useGravity = true;
    }

    public void Exit(ScoutDroidController agent)
    {
        return;
    }

    public AiFlyStateId GetId()
    {
        return AiFlyStateId.Death;
    }

    public void Update(ScoutDroidController agent)
    {
        return;
    }
}
