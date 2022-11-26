using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AiAttackState : AiState
{
    Vector3 wayPoint;
    
    bool rollActive = true;
    const float STRAFE_COOL_DOWN = 2f;
    const float WAIT = 0.5f;


    const float ROLL_COOL_DOWN = 3f;
    const float GRENADE_COOL_DOWN = 5f;
    float grenadeTimer = GRENADE_COOL_DOWN;

    float rollTimer = ROLL_COOL_DOWN;
    float strafeTimer = STRAFE_COOL_DOWN;
    float waitTimer = STRAFE_COOL_DOWN;
    bool waitActive = false;
    bool grenadeActive = true;

    bool strafeActive = true;

    void AiState.Enter(EnemyController enemyController)
    {
        // glitchy
        enemyController.Attack();
        enemyController.Aim();
        enemyController.animator.SetBool("Attack", true);
        wayPoint = enemyController.transform.position;
        enemyController.agent.speed = enemyController.strafeSpeed;
    }

    void AiState.Exit(EnemyController enemyController)
    {
        enemyController.StopAttack();
        enemyController.StopAim();
        enemyController.agent.speed = enemyController.chaseSpeed;
    }

    AiStateId AiState.GetId()
    {
        return AiStateId.Attack;
    }

    void AiState.Update(EnemyController enemyController)
    {
        // check if player is aiming in direction of enemy

        Vector3 playerForward = enemyController.targetOrientation.forward;
        Vector3 toEnemy = enemyController.transform.position - enemyController.target.transform.position;
        float angle = Vector3.Angle(playerForward, toEnemy);
        if (enemyController.grenades > 0 && grenadeActive)
        {
            if (Random.Range(0, 3) == 1)
            {
                enemyController.animator.SetTrigger("ThrowGrenade");
            }
            grenadeActive = false;
        }

        if (!rollActive)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer < 0)
            {
                rollTimer = 10;
                rollActive = true;
            }
        }

        if (!grenadeActive)
        {
            grenadeTimer -= Time.deltaTime;
            if (grenadeTimer < 0)
            {
                grenadeTimer = GRENADE_COOL_DOWN;
                grenadeActive = true;
            }
        }

        if (!rollActive)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer < 0)
            {
                rollTimer = ROLL_COOL_DOWN;
                rollActive = true;
            }
        }

        if (!strafeActive)
        {
            strafeTimer -= Time.deltaTime;
            if (strafeTimer < 0)
            {
                strafeTimer = STRAFE_COOL_DOWN;
                strafeActive = true;
            }
        }
        float distance = Vector3.Distance(enemyController.transform.position, wayPoint);

        if (angle < 8.0 && rollActive && distance < 0.025f)
        {
            enemyController.animator.SetTrigger("Roll");
            rollActive = false;
        }

        if (distance < 1.0f && waitActive)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer < 0)
            {
                waitTimer = WAIT;
                waitActive = false;
            }
        }
        if (strafeActive && !waitActive)
        {
            Vector3 randomVar = Vector3.zero;
            while (randomVar.magnitude < 7)
            {
                randomVar = Random.insideUnitSphere;
                randomVar *= 20.0f;
                randomVar.y = 0;
            }
          
            wayPoint = enemyController.transform.position + randomVar;
            enemyController.agent.SetDestination(wayPoint);
            strafeActive = false;
            waitActive = true;
       
        }

        


        enemyController.FaceTarget();
        Vector3 playerDir = enemyController.target.transform.position - enemyController.transform.position;
        if (playerDir.magnitude > enemyController.shootingRange)
        {
         
            enemyController.stateMachine.ChangeState(AiStateId.Chase);
        }

    }
}
