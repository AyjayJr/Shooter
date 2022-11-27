using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiIdleState : AiState
{
    public void Enter(EnemyController enemyController)
    {
        enemyController.StopAim();  
    }

    public void Exit(EnemyController enemyController)
    {
        return;
    }

    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }

    public void Update(EnemyController enemyController)
    {
        Vector3 playerDirection = enemyController.target.transform.position - enemyController.transform.position;
        if (playerDirection.magnitude > enemyController.visionRadius)
        {
            return;
        }
        if (playerDirection.magnitude < enemyController.autoAlertRadius)
        {
            enemyController.stateMachine.ChangeState(AiStateId.Chase);
        }
        Vector3 agentDirection = enemyController.transform.forward;
        playerDirection.Normalize();
        float dotProduct = Vector3.Dot(playerDirection, agentDirection);
       
        if (dotProduct > 0)
        {
            enemyController.stateMachine.ChangeState(AiStateId.Chase);
        }
    }
}
