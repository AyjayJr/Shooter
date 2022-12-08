using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Canvas loseScreen;
    [Header("Settings")]
    public bool toggleSprint;
    public bool disableScrollWheel;

    public Action<bool> onPaused;
    public Action onRespawn;
    public Action onLose;
    private bool isPaused = true;

    public bool IsPaused { get => isPaused; }
    private Transform respawnLocation;

    public void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "Intro")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isPaused = false;
            SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.GameplayDNB, true);
        }
        if (SceneManager.GetActiveScene().name == "Credits")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isPaused = false;
            SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.Credits);
        }
        toggleSprint = PlayerPrefs.GetInt("ToggleSprint") == 0 ? false : true;
        disableScrollWheel = PlayerPrefs.GetInt("DisableScrollWheel") == 0 ? false : true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Checkpoint.onCheckpointReached += (cp) => respawnLocation = cp;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "Intro")
            SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.GameplayDNB, true);
    }

    public void TogglePause(bool shouldShowPauseUI = false)
    {
        isPaused = !isPaused;
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
        TimeManager.Instance.PauseTimer();
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
        TimeManager.Instance.ResumeTimer();
        PostProcessVolume v = Camera.main.GetComponent<PostProcessVolume>();
        var vignette = v.profile.GetSetting<Vignette>();
        vignette.intensity.value = 0f;
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
