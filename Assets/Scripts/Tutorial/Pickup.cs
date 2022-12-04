using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Pickup : MonoBehaviour
{
    public static Action<PickedUpType> onPickup;

    public enum PickedUpType
    {
        GuassCannon,
        AssaultRifle
    }

    [SerializeField] private string title;
    [SerializeField] private PickedUpType pickedUpType;
    [SerializeField] private AudioClip pickupSFX;
    [SerializeField] private VideoClip tutVideo;
    [SerializeField] private RenderTexture renderTexture;
    [TextArea]
    [SerializeField] private string description;

    private bool oneAndDone = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !oneAndDone)
        {
            oneAndDone = true;
            onPickup?.Invoke(pickedUpType);
            if (pickupSFX)
                SoundManager.Instance.PlaySFXOnce(pickupSFX);
            UIManager.Instance.CreatePopup()?.ShowAsVertical(title, tutVideo, renderTexture, description, () => Debug.Log("Confirm"));
            Destroy(this.gameObject);
        }
    }
}
