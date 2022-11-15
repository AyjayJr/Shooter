using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Instantiate(winScreen, null);
            SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.WinSound);
            GameManager.Instance.WinScreen();
        }
    }
}
