using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pixelplacement;
using System;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private Button restartButton;
    [SerializeField] private LevelDataSO levelDataSO;
    [SerializeField] private Image foodImage;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timeNextRank;

    [Header("Animation Controls")]
    [SerializeField] private AnimationCurve tweenControl;
    [SerializeField] private float duration;
    [SerializeField] private float delay;

    private Scene currentScene;
    private LevelDataSO.Level currentLevel;
    private LevelDataSO.LevelRanks playerRank;

    void Start()
    {
        restartButton.onClick.AddListener(Restart);
        // temp go to main menu
        nextButton.onClick.AddListener(NextButtonClicked);
        currentScene = SceneManager.GetActiveScene();
        nextButton.interactable = false;
        nextText.text = "...";

        TimeManager.Instance.EndTimer();
        Tween.LocalScale(foodImage.transform, Vector3.zero, Vector3.one, duration, delay, tweenControl, Tween.LoopType.None, completeCallback: UpdateText);
        foodImage.sprite = DetermineFoodRank();
        timerText.text = TimeManager.Instance.TimerCounter.text;
        PlayerManager.Instance.isAlive = false;
        SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.Victory);
    }

    private void Restart()
    {
        GameManager.Instance.TogglePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void NextButtonClicked()
    {
        GameManager.Instance.TogglePause();
        SceneManager.LoadScene(0);
    }

    private Sprite DetermineFoodRank()
    {
        float bestTime = 9999;
        Sprite bestRank = null;
        string rankName = "";
        foreach (LevelDataSO.Level level in levelDataSO.levels)
        {
            if (level.scene.ScenePath == currentScene.path)
                currentLevel = level;
        }
        for (int i = 0; i < currentLevel.levelTimes.Length; i++)
        {
            if (TimeManager.Instance.GetCurrentTimeInSeconds() <= currentLevel.levelTimes[i].rankTimesInSeconds && bestTime > currentLevel.levelTimes[i].rankTimesInSeconds)
            {
                bestTime = currentLevel.levelTimes[i].rankTimesInSeconds;
                bestRank = currentLevel.levelTimes[i].rankImage;
                rankName = currentLevel.levelTimes[i].rankName;
                foodText.text = currentLevel.levelTimes[i].foodFlavorText;
                try
                {
                    var time = TimeSpan.FromSeconds(currentLevel.levelTimes[i - 1].rankTimesInSeconds);
                    timeNextRank.text = "Time To Next Rank: " + time.ToString("mm':'ss'.'ff");
                }
                catch (System.IndexOutOfRangeException ex)
                {
                    Debug.LogWarning("No Better Time Found");
                    timeNextRank.text = "Best Rank Achieved!";
                }
            }
        }
        playerRank = new LevelDataSO.LevelRanks();
        playerRank.rankImage = bestRank;
        playerRank.rankName = rankName;
        playerRank.rankTimesInSeconds = (int)bestTime;
        return bestRank;
    }

    private void UpdateText()
    {
        nextButton.interactable = true;
        switch (playerRank.rankName)
        {
            case "S":
                {
                    nextText.text = "Devour & Continue";
                    break;
                }
            case "F":
                {
                    nextText.text = "\"Eat\" & Continue";
                    break;
                }
            case "A":
            case "C":
            default:
                {
                    nextText.text = "Eat & Continue";
                    break;
                }
        }
    }
}
