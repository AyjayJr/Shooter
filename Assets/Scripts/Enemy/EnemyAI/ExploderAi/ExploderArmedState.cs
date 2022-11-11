using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderArmedState : ExploderState
{
    void ExploderState.Enter(ExploderController enemyController)
    {
        Debug.Log("entering armed");
        enemyController.StartBlinking();
    }

    void ExploderState.Exit(ExploderController enemyController)
    {
        enemyController.StopBlinking();
    }

    ExploderStateId ExploderState.GetId()
    {
        return ExploderStateId.Armed;
    }

    void ExploderState.Update(ExploderController enemyController)
    {

        enemyController.agent.SetDestination(enemyController.target.position);
        float distance = Vector3.Distance(enemyController.transform.position, enemyController.target.transform.position);
        if (distance > enemyController.armingRange)
        {
            enemyController.stateMachine.ChangeState(ExploderStateId.Chase);
        }
        if (distance < enemyController.activationRadius)
        {
            enemyController.stateMachine.ChangeState(ExploderStateId.Death);
        }

    }
}
