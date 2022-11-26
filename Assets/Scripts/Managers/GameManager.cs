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
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isPaused = false;
            SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.GameplayDNB, true);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.GameplayDNB, true);
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

    public void LoseScreen()
    {
        Instantiate(loseScreen, null);
        TimeManager.Instance.EndTimer();
        PlayerManager.Instance.player.GetComponent<PlayerMovementAdvanced>().inputEnabled = false;
        isPaused = true;
        ManageCursorState();
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
