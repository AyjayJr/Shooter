using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingArea : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && PlayerManager.Instance.isAlive)
        {
            GameManager.Instance.LoseScreen();
        }
    }
}
