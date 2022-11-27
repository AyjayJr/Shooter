using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossStateID
{
    Attack,
} 

public interface BossState
{
    BossStateID GetId();
    void Enter(BossController agent);
    void Update(BossController agent);
    void Exit(BossController agent);

}