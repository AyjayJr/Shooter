using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiIdleState : AiState
{
    Vector3 wanderDest;

    public void Enter(EnemyController enemyController)
    {
        enemyController.wasChasingPlayer = false;
        enemyController.StopAim();
        enemyController.animator.SetBool("PlayerDead", false);
        enemyController.agent.updateRotation = true;
        enemyController.agent.speed = enemyController.strafeSpeed;

    }

    public void Exit(EnemyController enemyController)
    {
        enemyController.agent.updateRotation = false;
    }

    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }

    public void Update(EnemyController enemyController)
    {
        
        if (!enemyController.agent.hasPath || enemyController.agent.isStopped)
        {
            wanderDest = enemyController.sensor.GetRandomPoint() + enemyController.transform.position;
            enemyController.agent.SetDestination(wanderDest);
        }
      
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
