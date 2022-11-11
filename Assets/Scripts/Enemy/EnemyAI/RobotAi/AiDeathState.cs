using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDeathState : AiState
{
    public void Enter(EnemyController enemyController)
    {
        enemyController.animator.enabled = false;
        enemyController.setRigidbodyState(false);
        enemyController.setColliderState(true);
        enemyController.isDead = true;
    }

    public void Exit(EnemyController agent)
    {
        return;
    }

    public AiStateId GetId()
    {
        return AiStateId.Death;
    }

    public void Update(EnemyController agent)
    {
        return;
    }
}
