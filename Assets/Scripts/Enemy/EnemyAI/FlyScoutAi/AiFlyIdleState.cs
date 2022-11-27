using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFlyIdleState : FlyState
{
    public void Enter(ScoutDroidController enemyController)
    {
        Material[] materials = enemyController.sphereRef.materials;
        materials[3] = enemyController.eyeEmmisionMaterials[0];
        materials[4] = enemyController.eyeEmmisionMaterials[1];
        enemyController.sphereRef.materials = materials;
    }

    public void Exit(ScoutDroidController agent)
    {
        return;
    }

    public AiFlyStateId GetId()
    {
        return AiFlyStateId.Idle;
    }

    public void Update(ScoutDroidController enemyController)
    {
        if (!enemyController.isFlying)
            enemyController.transform.position = Vector3.MoveTowards(enemyController.transform.position, 
                new Vector3(enemyController.transform.position.x, enemyController.transform.position.y + enemyController.YOffset, enemyController.transform.position.z), enemyController.speed * Time.deltaTime);
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
            enemyController.stateMachine.ChangeState(AiFlyStateId.Chase);
        }
    }
}
