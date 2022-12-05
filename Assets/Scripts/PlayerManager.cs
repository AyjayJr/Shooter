using UnityEngine;
using Pixelplacement;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager> //  <-- Has Instance From Singleton Class
{
    public GameObject player;
    public Transform orientation;

    public Action<float> onPlayerDamaged;
    public Action<float> onPlayerRegen;
    public bool isAlive = true;
    [SerializeField] private float Health;
    [SerializeField] private float lastDamageTaken = float.MaxValue;
    [SerializeField] private float healthRegenRate;
    
    private float maxHealth;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    void Awake()
    {
        maxHealth = Health;
    }

    private void Start()
    {
        GameManager.Instance.onRespawn += () => Health = maxHealth - 1;
    }

    public void LoseHealth(float damageReceived)
    {

        Health -= damageReceived;
        onPlayerDamaged?.Invoke(damageReceived);

        lastDamageTaken = Time.time + 3f;
        if (Health <= 0 && isAlive)
        {
            Debug.Log("player died");
            // Invoke tells subscribers to trigger listened functions (+=)
            GameManager.Instance.LoseScreen();
            return;
        }

        StopCoroutine(HealthRegneration());
    }

    void Update()
    {
        if (Health >= maxHealth || !isAlive)
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
        onPlayerRegen?.Invoke(healthRegenRate);

        if (Health >= maxHealth)
        {
            Health = maxHealth;
        }
        yield return null;
    }
}
