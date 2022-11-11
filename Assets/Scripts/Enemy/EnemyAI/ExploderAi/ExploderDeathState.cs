using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderDeathState : ExploderState
{
    void ExploderState.Enter(ExploderController enemyController)
    {
        enemyController.Explode();
    }

    void ExploderState.Exit(ExploderController enemyController)
    {
        
    }

    ExploderStateId ExploderState.GetId()
    {
        return ExploderStateId.Death;
    }

    void ExploderState.Update(ExploderController enemyController)
    {
        
    }
}
