using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Action<bool> onPaused;
    private bool isPaused = true;

    public bool IsPaused { get => isPaused; }

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
