using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private StateMachine stateMachineUI;
 
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Button backButton;

    private void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        settingsButton.onClick.AddListener(ShowSettings);
        backButton.onClick.AddListener(ShowMainMenu);

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        masterSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMasterVolume(x));
        musicSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMusicVolume(x));
        sfxSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetSFXVolume(x));
        SoundManager.Instance.StopAll();
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    private void ShowSettings()
    {
        stateMachineUI.ChangeState(settingsPanel);
    }

    private void ShowMainMenu()
    {
        stateMachineUI.ChangeState(main);
    }
}
