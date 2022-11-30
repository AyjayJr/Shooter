using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmQuitPanel;
    [SerializeField] private StateMachine stateMachineUI;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Start()
    {
        SoundManager.Instance.AddButtonSounds(resumeButton, SoundManager.GameSounds.MenuPlaySound);
        SoundManager.Instance.AddButtonSounds(restartButton, SoundManager.GameSounds.MenuBackSound);
        SoundManager.Instance.AddButtonSounds(settingsButton, SoundManager.GameSounds.MenuInputSound);
        SoundManager.Instance.AddButtonSounds(quitButton, SoundManager.GameSounds.MenuBackSound);

        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameManager.Instance.TogglePause();
        });
        settingsButton.onClick.AddListener(ShowSettings);
        quitButton.onClick.AddListener(ShowConfirmQuit);

        // actually quit
        yesButton.onClick.AddListener(QuitToMainMenu);
        noButton.onClick.AddListener(ShowPauseMenu);

        GameManager.Instance.onPaused += TogglePauseMenu;
        this.gameObject.SetActive(false);
    }

    private void TogglePauseMenu(bool showUI)
    {
        this.gameObject.SetActive(showUI);
        if (GameManager.Instance.IsPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    private void ResumeGame()
    {
        title.text = "Paused";
        GameManager.Instance.TogglePause();
    }

    private void ShowSettings()
    {
        title.text = "Settings";
        stateMachineUI.ChangeState(settingsPanel);
    }

    public void ShowPauseMenu()
    {
        title.text = "Paused";
        stateMachineUI.ChangeState(main);
    }

    private void ShowConfirmQuit()
    {
        title.text = "Are you sure you want to quit?";
        stateMachineUI.ChangeState(confirmQuitPanel);
    }

    private void QuitToMainMenu()
    {
        title.text = "Paused";
        this.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
