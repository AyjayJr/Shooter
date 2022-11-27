using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChaseState : AiState
{
    void AiState.Enter(EnemyController enemyController)
    {
        enemyController.Aim();
        enemyController.agent.speed = enemyController.chaseSpeed;
    }

    void AiState.Exit(EnemyController enemyController)
    {
        enemyController.agent.SetDestination(enemyController.transform.position);
    }

    AiStateId AiState.GetId()
    {
        return AiStateId.Chase;
    }

    void AiState.Update(EnemyController enemyController)
    {
        if (!enemyController.isDead)
        {
            enemyController.agent.SetDestination(enemyController.target.position);
            enemyController.FaceTarget();
        }

        Vector3 playerDirection = enemyController.target.transform.position - enemyController.transform.position;
        if (playerDirection.magnitude < enemyController.shootingRange)
        {
            enemyController.stateMachine.ChangeState(AiStateId.Attack);
        }
        if (playerDirection.magnitude > enemyController.visionRadius)
        {
            enemyController.stateMachine.ChangeState(AiStateId.Idle);
        }

    }
}
