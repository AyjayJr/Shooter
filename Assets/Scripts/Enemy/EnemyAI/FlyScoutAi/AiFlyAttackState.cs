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
        if (!enemyController.isAboveTarget && enemyController.transform.position.y < enemyController.target.position.y + enemyController.YOffset)
            enemyController.transform.position = Vector3.MoveTowards(enemyController.transform.position,
                new Vector3(enemyController.transform.position.x, enemyController.target.position.y + enemyController.YOffset, enemyController.transform.position.z), enemyController.speed * Time.deltaTime);
        enemyController.FaceTarget();

        Vector3 playerDirection = enemyController.target.transform.position - enemyController.transform.position;
        if (playerDirection.magnitude > enemyController.shootingRange)
        {
            enemyController.stateMachine.ChangeState(AiFlyStateId.Chase);
        }

    }
}
