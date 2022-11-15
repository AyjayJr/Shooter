using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pixelplacement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private Button restartButton;
    [SerializeField] private LevelDataSO levelDataSO;
    [SerializeField] private Image foodImage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;

    [Header("Animation Controls")]
    [SerializeField] private AnimationCurve tweenControl;
    [SerializeField] private float duration;
    [SerializeField] private float delay;

    private LevelDataSO.LevelRanks playerRank;

    void Start()
    {
        restartButton.onClick.AddListener(Restart);
        // temp go to main menu
        nextButton.onClick.AddListener(NextButtonClicked);
        nextButton.interactable = false;
        nextText.text = "...";

        TimeManager.Instance.EndTimer();
        Tween.LocalScale(foodImage.transform, Vector3.zero, Vector3.one, duration, delay, tweenControl, Tween.LoopType.None, completeCallback: UpdateText);
        foodImage.sprite = DetermineFoodRank();
        timerText.text = TimeManager.Instance.TimerCounter.text;
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
        for (int i = 0; i < levelDataSO.levelTimes.Length; i++)
        {
            if (TimeManager.Instance.GetCurrentTimeInSeconds() <= levelDataSO.levelTimes[i].rankTimesInSeconds && bestTime > levelDataSO.levelTimes[i].rankTimesInSeconds)
            {
                bestTime = levelDataSO.levelTimes[i].rankTimesInSeconds;
                bestRank = levelDataSO.levelTimes[i].rankImage;
                rankName = levelDataSO.levelTimes[i].rankName;
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
