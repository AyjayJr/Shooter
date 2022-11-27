using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayerDeathState : BossState
{
    void BossState.Enter(BossController enemyController)
    {
        enemyController.animator.SetBool("PlayerDead", true);
    }

    void BossState.Exit(BossController enemyController)
    {
    }

    BossStateID BossState.GetId()
    {
        return BossStateID.PlayerDeath;
    }

    void BossState.Update(BossController enemyController)
    {


    }
}
