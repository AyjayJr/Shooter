using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AiFleeState : AiState
{
    Vector3 fleeDestination;

    void AiState.Enter(EnemyController enemyController)
    {
        enemyController.agent.speed = enemyController.fleeingSpeed;
        fleeDestination = (-enemyController.transform.forward * enemyController.fleeDistance) + enemyController.transform.position;
        enemyController.agent.updateRotation = true;
        enemyController.agent.SetDestination(fleeDestination);
    }

    void AiState.Exit(EnemyController enemyController)
    {
        return;
    }

    AiStateId AiState.GetId()
    {
        return AiStateId.Flee;
    }

    void AiState.Update(EnemyController enemyController)
    {
        if (!enemyController.agent.hasPath)
        {
            enemyController.animator.SetTrigger("Roll");
            if (enemyController.wasChasingPlayer)
            {
                enemyController.stateMachine.ChangeState(AiStateId.Chase);
            }
            else
            {
                enemyController.stateMachine.ChangeState(AiStateId.Idle);
            }
        }
    }
}
