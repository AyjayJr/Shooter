using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemiesKilledText : MonoBehaviour
{
    private TextMeshProUGUI numEnemiesText;
    private int numEnemies = 0;
    private int deadEnemies = 0;

    void Awake()
    {
        numEnemiesText = GetComponent<TextMeshProUGUI>();
        EnemyController.onSpawned += AddToText;
        ExploderController.onSpawned += AddToText;
        ScoutDroidController.onSpawned += AddToText;

        EnemyController.onDeath += AddNumEnemiesDead;
        ExploderController.onDeath += AddNumEnemiesDead;
        ScoutDroidController.onDeath += AddNumEnemiesDead;
    }

    void AddNumEnemiesDead(bool revived)
    {
        if (!revived)
            deadEnemies++;
        numEnemiesText.text = "Enemies Killed " + deadEnemies + "/" + numEnemies;
    }

    void AddToText()
    {
        numEnemies++;
        numEnemiesText.text = "Enemies Killed " + deadEnemies + "/" + numEnemies;
    }

    private void OnDestroy()
    {
        EnemyController.onSpawned -= AddToText;
        ExploderController.onSpawned -= AddToText;
        ScoutDroidController.onSpawned -= AddToText;

        EnemyController.onDeath -= AddNumEnemiesDead;
        ExploderController.onDeath -= AddNumEnemiesDead;
        ScoutDroidController.onDeath -= AddNumEnemiesDead;
    }
}
