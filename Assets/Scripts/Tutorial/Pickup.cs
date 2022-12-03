using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public static Action onPickup;
    [SerializeField] private AudioClip pickupSFX;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onPickup?.Invoke();
            if (pickupSFX)
                SoundManager.Instance.PlaySFXOnce(pickupSFX);
            Destroy(this.gameObject);
        }
    }
}
