using UnityEngine;
using Pixelplacement;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager> //  <-- Has Instance From Singleton Class
{
    public GameObject player;
    public Action onPlayerDamaged;
    public Action onPlayerDeath;

    [SerializeField] private float Health = 100;
    [SerializeField] private float lastDamageTaken = float.MaxValue;
    [SerializeField] private float healthRegenRate;

    private float internalHealthCounter;


    void Awake()
    {
        internalHealthCounter = Health;
    }

    public void LoseHealth(float damageReceived)
    {
        if (Health < 0) return;

        StopCoroutine(HealthRegneration());

        Health -= damageReceived;
        onPlayerDamaged?.Invoke();

        lastDamageTaken = Time.time + 3f;
    }

    void Update()
    {
        if (Health <= 0f)
        {
            // Invoke tells subscribers to trigger listened functions (+=)
            onPlayerDeath?.Invoke();
            SceneManager.LoadScene(0);
        }

        if (Time.time >= lastDamageTaken)
        {
            lastDamageTaken = float.MaxValue;
            StartCoroutine(HealthRegneration());
        }
    }

    IEnumerator HealthRegneration()
    {
        while (true)
        {
            Health += healthRegenRate;
            Mathf.Clamp(Health, 0, internalHealthCounter);

            if (Health >= internalHealthCounter)
            {
                Health = internalHealthCounter;
                break;
            }

            yield return null;
        }
    }
}
