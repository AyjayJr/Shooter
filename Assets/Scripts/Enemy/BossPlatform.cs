using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlatform : MonoBehaviour
{
    public float damage = 4f;
    bool playerOn = false;
    bool damageActive = true;

    void Update()
    {
        if (playerOn && damageActive)
        {
            PlayerManager.Instance.LoseHealth(damage * Time.deltaTime);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        PlayerTarget player = collision.collider.GetComponent<PlayerTarget>();
        if (player != null)
        {
            playerOn = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerTarget player = collision.collider.GetComponent<PlayerTarget>();
        if (player != null)
        {
            playerOn = false;
        
        }
    }

    public void DamageOff()
    {
        damageActive = false;
    }
}
