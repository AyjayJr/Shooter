using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeathState : BossState
{
    public void Enter(BossController enemyController)
    {
        enemyController.animator.enabled = false;
        enemyController.isDead = true;
    }

    public void Exit(BossController agent)
    {
        return;
    }

    public BossStateID GetId()
    {
        return BossStateID.Death;
    }

    public void Update(BossController agent)
    {
        return;
    }
}
