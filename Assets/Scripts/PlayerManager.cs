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
    [SerializeField] private float hurtSoundCD = 1f;
    
    private float maxHealth;
    private bool canPlayHurtSound = true;
    private float hurtCooldown = -1;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    void Awake()
    {
        maxHealth = Health;
    }

    private void Start()
    {
        GameManager.Instance.onRespawn += () => Health = maxHealth - 3;
        hurtCooldown = -1;
    }

    public void LoseHealth(float damageReceived)
    {

        Health -= damageReceived;
        if (canPlayHurtSound && !GameManager.Instance.IsPaused)
        {
            SoundManager.Instance.PlayRandomPlayerHurt();
            hurtCooldown = hurtSoundCD;
        }
            
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

        if (hurtCooldown <= 0.0f)
        {
            canPlayHurtSound = true;
        }  
        else
        {
            hurtCooldown -= Time.deltaTime;
            canPlayHurtSound = false;
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
