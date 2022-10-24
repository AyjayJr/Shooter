
using UnityEngine;
using Pixelplacement;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>
{
    #region Singleton
       
    public static PlayerManager Instance;
    [SerializeField] private float Health = 100;
    [SerializeField] private float lastDamageTaken = float.MaxValue;
    [SerializeField] private float healthRegenRate;

    private float internalHealthCounter;


    void Awake()
    {
        Instance = this;
        internalHealthCounter = Health;
    }

    public void LoseHealth(float damageReceived)
    {
        if(Health < 0) return;

        StopAllCoroutines();
        
        Health -= damageReceived;

        lastDamageTaken = Time.time + 3f;
    }

    void Update()
    {
        if(Health <= 0f)
        {
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
        while(true)
        {
            Health += healthRegenRate;
            Mathf.Clamp(Health, 0, internalHealthCounter);

            if(Health >= internalHealthCounter)
            {
                Health = internalHealthCounter;
                break;
            }

            yield return null;
        }
    }

    #endregion

    public GameObject player;
}
