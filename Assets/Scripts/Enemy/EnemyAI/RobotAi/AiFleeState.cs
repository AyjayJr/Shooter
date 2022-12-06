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
        Debug.Log("flee");

    }

    void AiState.Exit(EnemyController enemyController)
    {
        Debug.Log("exit flee");

    }

    AiStateId AiState.GetId()
    {
        return AiStateId.Flee;
    }

    void AiState.Update(EnemyController enemyController)
    {
        Debug.Log("fleeing");

        if (!enemyController.agent.hasPath)
        {
            Debug.Log("no path");

            enemyController.animator.SetTrigger("Roll");
            if (enemyController.wasChasingPlayer)
            {
                Debug.Log("going chase");
                enemyController.stateMachine.ChangeState(AiStateId.Chase);
            }
            else
            {
                Debug.Log("going idle");

                enemyController.stateMachine.ChangeState(AiStateId.Idle);
            }
        }
    }
}
