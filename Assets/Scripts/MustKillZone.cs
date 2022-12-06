using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MustKillZone : MonoBehaviour
{
    public GameObject blockade;
    public GameObject enemyGroup;
    private int numOfEnemies;
    private int numOfAliveEnemies;
    public Transform[] enemysArray;
    private int[] aliveEnemiesArray;

    //REFERENCE THIS VARIABLE TO CHECK IF ALL FLOORS HAVE BEEN CLEARED
    public bool FloorHasBeenCleared;
    private bool active;

    void Awake()
    {
        FloorHasBeenCleared = false;
        active = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        numOfEnemies = enemyGroup.transform.childCount;
        numOfAliveEnemies = numOfEnemies;

        aliveEnemiesArray = new int[numOfEnemies];

        //Set all indices of enemies to 1, to show that they are currently alive. 0 means dead
        for(int i = 0; i < numOfEnemies; i++)
        {
            aliveEnemiesArray[i] = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(numOfAliveEnemies == 0 && !FloorHasBeenCleared)
        {
            Destroy(blockade);

            Debug.Log(this.gameObject.name + " has been cleared");

            FloorHasBeenCleared = true;

            active = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("KILL EVERYONE");
            active = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(active)
        {
            for(int i = 0; i < numOfEnemies; i++)
            {
                //if regular enemy: true false false. if exploder: false false true. if scout: false true false
                if(NullToFalse_Enemy(enemysArray[i]) || NullToFalse_Scout(enemysArray[i]) || NullToFalse_Exploder(enemysArray[i]))
                {
                    //check if this enemy died earlier
                    if(aliveEnemiesArray[i] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        numOfAliveEnemies--;
                        aliveEnemiesArray[i] = 0;
                        //Destroy(enemysArray[i]);
                    }
                }
            }
        }
    }

    //There is probably a better way to do this, but i need a solution fast
    //three separate checks to convert null - as in the enemy is not that type - to false to prevent errors
    bool NullToFalse_Enemy(Transform enemy)
    {
        if(enemy.gameObject.GetComponent<EnemyController>() == null)
        {
            return false;
        }
        
        if(enemy.gameObject.GetComponent<EnemyController>().isDead)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool NullToFalse_Scout(Transform enemy)
    {
        if(enemy.gameObject.GetComponent<ScoutDroidController>() == null)
        {
            return false;
        }
        
        if(enemy.gameObject.GetComponent<ScoutDroidController>().isDead)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool NullToFalse_Exploder(Transform enemy)
    {
        if(enemy.gameObject.GetComponentInChildren<ExploderController>() == null)
        {
            return false;
        }
        
        if(enemy.gameObject.GetComponentInChildren<ExploderController>().isDead)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
