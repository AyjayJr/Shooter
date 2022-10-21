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
    [SerializeField] private Sprite[] foodSprites;
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

        Tween.LocalScale(foodImage.transform, Vector3.zero, Vector3.one, duration, delay, tweenControl, Tween.LoopType.None);
        TimeManager.Instance.EndTimer();
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
}
