using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderPatrolState : ExploderState
{
    WayPoint wayPoint;

    void ExploderState.Enter(ExploderController enemyController)
    {
        wayPoint = enemyController.NextWayPoint(0);
        Debug.Log("new waypoint first" + wayPoint.id);
        enemyController.agent.SetDestination(wayPoint.transform.position);
    }

    void ExploderState.Exit(ExploderController enemyController)
    {
        // enemyController.agent.SetDestination(enemyController.transform.position);
    }

    ExploderStateId ExploderState.GetId()
    {
        return ExploderStateId.Patrol;
    }

    void ExploderState.Update(ExploderController enemyController)
    {
        float distance = Vector3.Distance(enemyController.transform.position, wayPoint.transform.position);
        if (!enemyController.agent.hasPath || distance < 2.2f)
        {
            Debug.Log("Waypoint ID" + wayPoint.id);
            wayPoint = enemyController.NextWayPoint(wayPoint.id);
            Debug.Log("new waypoint " + wayPoint.id);
            enemyController.agent.SetDestination(wayPoint.transform.position);
        }
       
        Vector3 playerDirection = enemyController.target.transform.position - enemyController.transform.position;
        if (playerDirection.magnitude > enemyController.visionRadius)
        {
            return;
        }

        Vector3 agentDirection = enemyController.transform.forward;
        playerDirection.Normalize();
        float dotProduct = Vector3.Dot(playerDirection, agentDirection);

        if (dotProduct > 0)
        {
            enemyController.stateMachine.ChangeState(ExploderStateId.Chase);
        }

    }
}
