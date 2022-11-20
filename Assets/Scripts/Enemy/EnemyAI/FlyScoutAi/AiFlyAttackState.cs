using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFlyAttackState : FlyState
{
    void FlyState.Enter(ScoutDroidController enemyController)
    {
        enemyController.Attack();
    }

    void FlyState.Exit(ScoutDroidController enemyController)
    {
        enemyController.StopAttack();
    }

    AiFlyStateId FlyState.GetId()
    {
        return AiFlyStateId.Attack;
    }

    void FlyState.Update(ScoutDroidController enemyController)
    {

        enemyController.FaceTarget();

        Vector3 playerDirection = enemyController.target.transform.position - enemyController.transform.position;
        if (playerDirection.magnitude > enemyController.shootingRange)
        {
            enemyController.stateMachine.ChangeState(AiFlyStateId.Chase);
        }

    }
}
