using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderChaseState : ExploderState
{
    void ExploderState.Enter(ExploderController enemyController)
    {
    }

    void ExploderState.Exit(ExploderController enemyController)
    {
        // enemyController.agent.SetDestination(enemyController.transform.position);
    }

    ExploderStateId ExploderState.GetId()
    {
        return ExploderStateId.Chase;
    }

    void ExploderState.Update(ExploderController enemyController)
    {
        enemyController.agent.SetDestination(enemyController.target.position);
        float distance = Vector3.Distance(enemyController.transform.position, enemyController.target.transform.position);
        if (distance < enemyController.armingRange)
        {
            enemyController.stateMachine.ChangeState(ExploderStateId.Armed);
        }

    }
}
