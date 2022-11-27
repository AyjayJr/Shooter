using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlatform : MonoBehaviour
{
    public float damage = 4f;
    bool playerOn = false;

    void Update()
    {
        if (playerOn)
        {
            PlayerManager.Instance.LoseHealth(damage * Time.deltaTime);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("colis");
        PlayerTarget player = collision.collider.GetComponent<PlayerTarget>();
        if (player != null)
        {
            Debug.Log("player on");
            playerOn = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerTarget player = collision.collider.GetComponent<PlayerTarget>();
        if (player != null)
        {
            Debug.Log("player off");
            playerOn = false;
        
        }
    }
}
