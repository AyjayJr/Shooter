using UnityEngine;
using Pixelplacement;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager> //  <-- Has Instance From Singleton Class
{
    public GameObject player;
    public Action<float> onPlayerDamaged;
    public Action onPlayerDeath;
    public Action<float> onPlayerRegen;

    [SerializeField] private float Health;
    [SerializeField] private float lastDamageTaken = float.MaxValue;
    [SerializeField] private float healthRegenRate;

    private float maxHealth;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    void Awake()
    {
        maxHealth = Health;
    }

    public void LoseHealth(float damageReceived)
    {
        if (Health < 0) return;

        StopCoroutine(HealthRegneration());

        Health -= damageReceived;
        onPlayerDamaged?.Invoke(damageReceived);

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

        if (Health >= maxHealth)
        {
            StopCoroutine(HealthRegneration());
            return;
        }

        if (Time.time >= lastDamageTaken && Health < maxHealth)
        {
            lastDamageTaken = Time.time + 2f;
            StartCoroutine(HealthRegneration());
        }
    }

    IEnumerator HealthRegneration()
    {
        Health += healthRegenRate;
        Mathf.Clamp(Health, 0, maxHealth);
        Debug.Log("REGEN, current health: " + Health);
        onPlayerRegen?.Invoke(healthRegenRate);

        if (Health >= maxHealth)
        {
            Health = maxHealth;
        }
        yield return null;
    }
}
