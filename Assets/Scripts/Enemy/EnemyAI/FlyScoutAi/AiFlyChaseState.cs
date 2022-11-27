using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFlyChaseState : FlyState
{
    void FlyState.Enter(ScoutDroidController enemyController)
    {
        enemyController.FaceTarget();

        // bad way to do this but fuck it, there's no time
        Material[] materials = enemyController.sphereRef.materials;
        materials[3] = enemyController.eyeEmmisionMaterials[2];
        materials[4] = enemyController.eyeEmmisionMaterials[3];
        enemyController.sphereRef.materials = materials;
    }

    void FlyState.Exit(ScoutDroidController enemyController)
    {
        return;
    }

    AiFlyStateId FlyState.GetId()
    {
        return AiFlyStateId.Chase;
    }

    void FlyState.Update(ScoutDroidController enemyController)
    {
        Vector3 playerDirection = enemyController.target.transform.position - enemyController.transform.position;
        if (!enemyController.isDead)
        {
            //enemyController.agent.SetDestination(enemyController.target.position);
            enemyController.FaceTarget();
            Vector3 targetPos = new Vector3(enemyController.target.position.x, enemyController.target.position.y + enemyController.YOffset, enemyController.target.position.z);
            enemyController.transform.position = Vector3.MoveTowards(enemyController.transform.position, targetPos, enemyController.speed * Time.deltaTime);
        }

        if (playerDirection.magnitude < enemyController.shootingRange)
        {
            enemyController.stateMachine.ChangeState(AiFlyStateId.Attack);
        }
        if (playerDirection.magnitude > enemyController.visionRadius)
        {
            enemyController.stateMachine.ChangeState(AiFlyStateId.Idle);
        }

    }
}
