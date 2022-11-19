using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiStateId
{
    Idle,
    Chase,
    Death,
    Attack,
    PlayerDeath
} 

public interface AiState
{
    AiStateId GetId();
    void Enter(EnemyController agent);
    void Update(EnemyController agent);
    void Exit(EnemyController agent);

}