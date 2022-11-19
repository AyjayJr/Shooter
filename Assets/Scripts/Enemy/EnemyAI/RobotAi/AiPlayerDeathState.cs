using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerDeathState : AiState
{
    void AiState.Enter(EnemyController enemyController)
    {
        enemyController.animator.SetBool("PlayerDead", true);
    }

    void AiState.Exit(EnemyController enemyController)
    {
    }

    AiStateId AiState.GetId()
    {
        return AiStateId.PlayerDeath;
    }

    void AiState.Update(EnemyController enemyController)
    {


    }
}
