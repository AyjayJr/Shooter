using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : BossState
{
    const float THROW_COOL_DOWN = 7f;
    float throwTimer = THROW_COOL_DOWN;
    bool throwActive = false;

    void BossState.Enter(BossController enemyController)
    {
    }

    void BossState.Exit(BossController enemyController)
    {
    }

    BossStateID BossState.GetId()
    {
        return BossStateID.Attack;
    }

    void BossState.Update(BossController enemyController)
    {

        if (!throwActive)
        {
            throwTimer -= Time.deltaTime;
            if (throwTimer < 0)
            {
                throwTimer = THROW_COOL_DOWN;
                throwActive = true;
            }
        }

        if (throwActive)
        {
            throwActive = false;
            enemyController.animator.SetTrigger("ThrowStove");
        }

        enemyController.FaceTarget();
    }
}
