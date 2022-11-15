using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Canvas loseScreen;

    public Action<bool> onPaused;
    private bool isPaused = true;

    public bool IsPaused { get => isPaused; }

    public void Start()
    {
        if (SceneManager.GetActiveScene().name == "Map MVP")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isPaused = false;
        }
        PlayerManager.Instance.onPlayerDeath += LoseScreen;
    }

    public void TogglePause(bool shouldShowPauseUI = false)
    {
        isPaused = !isPaused;
        Debug.Log(IsPaused);
        onPaused?.Invoke(shouldShowPauseUI);
        ManageCursorState();
    }

    public void WinScreen()
    {
        isPaused = true;
        PlayerManager.Instance.player.GetComponent<PlayerMovementAdvanced>().inputEnabled = false;
        ManageCursorState();
    }

    private void LoseScreen()
    {
        Instantiate(loseScreen, null);
        PlayerManager.Instance.player.GetComponent<PlayerMovementAdvanced>().inputEnabled = false;
        TogglePause(false);
    }

    private void ManageCursorState()
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
