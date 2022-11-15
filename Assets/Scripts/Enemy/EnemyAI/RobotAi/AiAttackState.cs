using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackState : AiState
{
    void AiState.Enter(EnemyController enemyController)
    {
        // glitchy
        enemyController.Attack();
        enemyController.Aim();
    }

    void AiState.Exit(EnemyController enemyController)
    {
        enemyController.StopAttack();
        enemyController.StopAim();
    }

    AiStateId AiState.GetId()
    {
        return AiStateId.Attack;
    }

    void AiState.Update(EnemyController enemyController)
    {

        enemyController.FaceTarget();

        Vector3 playerDirection = enemyController.target.transform.position - enemyController.transform.position;
        if (playerDirection.magnitude > enemyController.shootingRange)
        {
            enemyController.stateMachine.ChangeState(AiStateId.Chase);
        }

    }
}
