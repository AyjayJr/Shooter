using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExploderStateId
{
    Chase,
    Patrol,
    Armed,
    Death
} 

public interface ExploderState
{
    ExploderStateId GetId();
    void Enter(ExploderController agent);
    void Update(ExploderController agent);
    void Exit(ExploderController agent);

}