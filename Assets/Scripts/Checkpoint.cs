using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    public static Action<Transform> onCheckpointReached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onCheckpointReached?.Invoke(this.transform);
        }
    }
}
