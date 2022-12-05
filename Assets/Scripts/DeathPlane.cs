using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private bool oneAndDone = false;
    private void Start()
    {
        GameManager.Instance.onRespawn += () => oneAndDone = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !oneAndDone)
        {
            oneAndDone = true;
            GameManager.Instance.LoseScreen();
        }
    }
}
