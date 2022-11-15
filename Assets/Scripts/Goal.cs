using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    private bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            GameObject.Instantiate(winScreen, null);
            SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.WinSound);
            GameManager.Instance.WinScreen();
        }
    }
}
