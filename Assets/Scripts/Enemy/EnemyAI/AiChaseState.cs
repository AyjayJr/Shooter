using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChaseState : AiState
{
    void AiState.Enter(EnemyController agent)
    {
        return;
    }

    void AiState.Exit(EnemyController agent)
    {
        return;
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
    }
}
