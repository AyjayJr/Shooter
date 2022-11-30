using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private StateMachine stateMachineUI;
 
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        SoundManager.Instance.AddButtonSounds(playButton, SoundManager.GameSounds.MenuPlaySound);
        SoundManager.Instance.AddButtonSounds(settingsButton, SoundManager.GameSounds.MenuInputSound);
        SoundManager.Instance.AddButtonSounds(quitButton, SoundManager.GameSounds.MenuBackSound);

        playButton.onClick.AddListener(PlayGame);
        settingsButton.onClick.AddListener(ShowSettings);
        quitButton.onClick.AddListener(() => Application.Quit());
        
        SoundManager.Instance.PlayMusicLoop(SoundManager.MusicTracks.MainMenu);
    }

    private void PlayGame()
    {
        GameManager.Instance.TogglePause();
        SceneManager.LoadScene(1);
    }

    private void ShowSettings()
    {
        stateMachineUI.ChangeState(settingsPanel);
    }

    public void ShowMainMenu()
    {
        stateMachineUI.ChangeState(main);
    }
}
