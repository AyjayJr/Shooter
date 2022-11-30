using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private List<ResolutionOptions> resolutionOptions;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button resLeft;
    [SerializeField] private Button resRight;
    [SerializeField] private TextMeshProUGUI resoLabel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button apply;

    private int resolution = 0;

    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        qualityDropdown.value = PlayerPrefs.GetInt("Graphics");
        if (!PlayerPrefs.HasKey("Graphics"))
            PlayerPrefs.SetInt("Graphics", 3);

        SoundManager.Instance.AddButtonSounds(backButton, SoundManager.GameSounds.MenuBackSound);
        SoundManager.Instance.AddSliderSounds(musicSlider, SoundManager.GameSounds.MenuMusicSlider, true);
        SoundManager.Instance.AddSliderSounds(sfxSlider, SoundManager.GameSounds.MenuSFXSlider, false);
        SoundManager.Instance.AddSliderSounds(masterSlider, SoundManager.GameSounds.MenuMasterSlider, false);

        resLeft.onClick.AddListener(ChangeResLeft);
        resRight.onClick.AddListener(ChangeResRight);
        apply.onClick.AddListener(ApplyVideoSettings);
        fullscreenToggle.isOn = Screen.fullScreen;
        InitResolution();

        masterSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMasterVolume(x));
        musicSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetMusicVolume(x));
        sfxSlider.onValueChanged.AddListener((x) => SoundManager.Instance.SetSFXVolume(x));
    }

    private void InitResolution()
    {
        bool foundRes = false;
        for (int i = 0; i < resolutionOptions.Count; i++)
        {
            if (Screen.width == resolutionOptions[i].width && Screen.height == resolutionOptions[i].height)
            {
                foundRes = true;
                resolution = i;
                UpdateResLabel();
            }
        }

        if (!foundRes)
        {
            ResolutionOptions newResOption = new ResolutionOptions();
            newResOption.width = Screen.width;
            newResOption.height = Screen.height;

            resolutionOptions.Insert(0, newResOption);
            resolution = 0;
            UpdateResLabel();
        }
    }

    private void ChangeResLeft()
    {
        resolution--;
        if (resolution < 0)
            resolution = 0;

        UpdateResLabel();
    }

    private void ChangeResRight()
    {
        resolution++;
        if (resolution > resolutionOptions.Count - 1)
            resolution = resolutionOptions.Count - 1;

        UpdateResLabel();
    }

    private void UpdateResLabel()
    {
        resoLabel.text = resolutionOptions[resolution].width + " x " + resolutionOptions[resolution].height;
    }

    private void ApplyVideoSettings()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value, true);
        PlayerPrefs.SetInt("Graphics", qualityDropdown.value);

        Screen.SetResolution(resolutionOptions[resolution].width, resolutionOptions[resolution].height, fullscreenToggle.isOn);
    }

    [Serializable]
    public class ResolutionOptions
    {
        public int width;
        public int height;
    }
}
