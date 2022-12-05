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
    public Action onRespawn;
    public Action onLose;
    private bool isPaused = true;

    public bool IsPaused { get => isPaused; }
    private Transform respawnLocation;
    private Canvas loseScreenSpawned;

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
        Checkpoint.onCheckpointReached += (cp) => respawnLocation = cp;
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
        onLose?.Invoke();
        SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.Death);
        TimeManager.Instance.EndTimer();
        PlayerManager.Instance.player.GetComponent<PlayerMovementAdvanced>().inputEnabled = false;
        PlayerManager.Instance.isAlive = false;
        PlayerManager.Instance.player.GetComponent<Rigidbody>().isKinematic = true;
        isPaused = true;
        ManageCursorState();
    }

    public void RespawnPlayer()
    {
        // Find is bad but the deadline is soon so rip
        LoseScreen[] screens = FindObjectsOfType<LoseScreen>();
        foreach (LoseScreen s in screens)
            Destroy(s.gameObject);
        PlayerManager.Instance.player.GetComponent<PlayerMovementAdvanced>().inputEnabled = true;
        SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.GameplayDNB, true);
        PlayerManager.Instance.isAlive = true;
        PlayerManager.Instance.player.GetComponent<Rigidbody>().isKinematic = false;
        TimeManager.Instance.BeginTimer();
        onRespawn?.Invoke();
        TogglePause();
        if (respawnLocation)
            PlayerManager.Instance.player.transform.position = respawnLocation.position;
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
