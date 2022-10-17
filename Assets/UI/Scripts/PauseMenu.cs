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
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button backButton;

    private PlayerMovementAdvanced player;

    private void Start()
    {
        SoundManager.Instance.AddButtonSounds(resumeButton, SoundManager.GameSounds.MenuPlaySound);
        SoundManager.Instance.AddButtonSounds(settingsButton, SoundManager.GameSounds.MenuInputSound);
        SoundManager.Instance.AddButtonSounds(backButton, SoundManager.GameSounds.MenuBackSound);
        SoundManager.Instance.AddButtonSounds(quitButton, SoundManager.GameSounds.MenuBackSound);
        SoundManager.Instance.AddSliderSounds(musicSlider, SoundManager.GameSounds.MenuMusicSlider, true);
        SoundManager.Instance.AddSliderSounds(sfxSlider, SoundManager.GameSounds.MenuSFXSlider, false);
        SoundManager.Instance.AddSliderSounds(masterSlider, SoundManager.GameSounds.MenuMasterSlider, false);

        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(ShowSettings);
        backButton.onClick.AddListener(ShowPauseMenu);
        quitButton.onClick.AddListener(ShowConfirmQuit);

        // actually quit
        yesButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        noButton.onClick.AddListener(ShowPauseMenu);

        masterSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMasterVolume(x));
        musicSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMusicVolume(x));
        sfxSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetSFXVolume(x));

        GameManager.Instance.onPaused += TogglePauseMenu;
        this.gameObject.SetActive(false);
    }

    private void TogglePauseMenu(bool isPaused)
    {
        this.gameObject.SetActive(isPaused);
        if (isPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    private void ResumeGame()
    {
        title.text = "Paused";
        GameManager.Instance.isPaused = false;
        GameManager.Instance.onPaused?.Invoke(false);
    }

    private void ShowSettings()
    {
        title.text = "Settings";
        stateMachineUI.ChangeState(settingsPanel);
    }

    private void ShowPauseMenu()
    {
        title.text = "Paused";
        stateMachineUI.ChangeState(main);
    }

    private void ShowConfirmQuit()
    {
        title.text = "Are you sure you want to quit?";
        stateMachineUI.ChangeState(confirmQuitPanel);
    }
}
