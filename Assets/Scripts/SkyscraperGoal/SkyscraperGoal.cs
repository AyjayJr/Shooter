using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperGoal : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    private bool triggered;

    public Transform floor1;
    public Transform floor2;
    public Transform floor3;
    public Transform floor4;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered && checkAllFloorsAreCleared())
        {
            triggered = true;
            GameObject.Instantiate(winScreen, null);
            SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.WinSound);
            GameManager.Instance.WinScreen();
        }
        else
        {
            Debug.Log("Still have enemies to kill!");
        }
    }

    bool checkAllFloorsAreCleared()
    {
        if(floor1.gameObject.GetComponent<MustKillZone>().FloorHasBeenCleared
        && floor2.gameObject.GetComponent<MustKillZone>().FloorHasBeenCleared
        && floor3.gameObject.GetComponent<MustKillZone>().FloorHasBeenCleared
        && floor4.gameObject.GetComponent<MustKillZone>().FloorHasBeenCleared
        )
        {
            return true;
        }

        return false;
    }
}
