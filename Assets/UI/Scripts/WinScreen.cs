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
    [SerializeField] private Button restartButton;
    [SerializeField] private LevelDataSO levelDataSO;
    [SerializeField] private Image foodImage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;

    [Header("Animation Controls")]
    [SerializeField] private AnimationCurve tweenControl;
    [SerializeField] private float duration;
    [SerializeField] private float delay;

    void Start()
    {
        restartButton.onClick.AddListener(Restart);
        // temp go to main menu
        nextButton.onClick.AddListener(NextButtonClicked);

        TimeManager.Instance.EndTimer();
        Tween.LocalScale(foodImage.transform, Vector3.zero, Vector3.one, duration, delay, tweenControl, Tween.LoopType.None);
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
        for (int i = 0; i < levelDataSO.levelTimes.Length; i++)
        {
            if (TimeManager.Instance.GetCurrentTimeInSeconds() <= levelDataSO.levelTimes[i].rankTimesInSeconds && bestTime > levelDataSO.levelTimes[i].rankTimesInSeconds)
            {
                bestTime = levelDataSO.levelTimes[i].rankTimesInSeconds;
                bestRank = levelDataSO.levelTimes[i].rankImage;
            }
        }
        return bestRank;
    }
}
