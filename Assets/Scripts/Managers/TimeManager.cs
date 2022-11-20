using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class TimeManager : Singleton<TimeManager>
{
    [SerializeField] private GameObject levelTimerPrefab;

    private TextMeshProUGUI timerCounter;
    private TimeSpan timePlaying;
    private bool timerActive;
    private float elapsedTime;

    public TextMeshProUGUI TimerCounter { get => timerCounter; }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        if (SceneManager.GetActiveScene().name == "Map MVP")
        {
            InitializeTimer();
        }
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Map MVP")
        {
            InitializeTimer();
        }
    }

    private void InitializeTimer()
    {
        GameObject timerGO = Instantiate(levelTimerPrefab, null);
        timerCounter = timerGO.GetComponentInChildren<TextMeshProUGUI>();
        BeginTimer();
    }

    public void BeginTimer()
    {
        elapsedTime = 0f;
        timerActive = true;
    }

    private void Update()
    {
        if (timerActive)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingString = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            timerCounter.text = timePlayingString;
        }
    }

    public float GetCurrentTimeInSeconds()
    {
        return elapsedTime;
    }

    public void EndTimer()
    {
        timerActive = false;
    }
}
