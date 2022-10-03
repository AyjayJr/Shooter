using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class PauseMenu : MonoBehaviour
{
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
        playButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(ShowSettings);
        backButton.onClick.AddListener(ShowMainMenu);

        masterSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMasterVolume(x));
        musicSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMusicVolume(x));
        sfxSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetSFXVolume(x));
    }

    private void ResumeGame()
    {
        this.gameObject.SetActive(false);
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
