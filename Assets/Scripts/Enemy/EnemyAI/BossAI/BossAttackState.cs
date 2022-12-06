using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : BossState
{
    const float THROW_COOL_DOWN = 7f;
    const float QUICK_COOL_DOWN = 2f;

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
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    throwTimer = THROW_COOL_DOWN;
                }
                else
                {
                    throwTimer = QUICK_COOL_DOWN;
                }
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
