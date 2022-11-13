using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    public SoundAudioClip[] audioClipArray;
    public AudioClip[] footstepClipArray;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    public enum GameSounds
    {
        MenuHoverSound,
        MenuInputSound,
        MenuBackSound,
        MenuPlaySound,
        MenuMusicSlider,
        MenuSFXSlider,
        MenuMasterSlider
    }

    private List<AudioSource> audioSources;

    public void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
    }

    public void AddAudioSource(AudioSource audioSource)
    {
        audioSources.Add(audioSource);
    }

    public void AddButtonSounds(Button button, GameSounds onClickSound)
    {
        button.onClick.AddListener(() => SoundManager.Instance.PlaySFXOnce(onClickSound));
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((x) => SoundManager.Instance.PlaySFXOnce(GameSounds.MenuHoverSound));
        eventTrigger.triggers.Add(entry);
    }

    public void AddSliderSounds(Slider slider, GameSounds onValueChangedSound, bool isMusic)
    {
        if (isMusic)
            slider.onValueChanged.AddListener((x) => SoundManager.Instance.PlayMusicOnceNoInterrupt(onValueChangedSound));
        else
            slider.onValueChanged.AddListener((x) => SoundManager.Instance.PlaySFXOnceNoInterrupt(onValueChangedSound));
    }

    public void PlaySFXOnce(GameSounds sound)
    {
        sfxAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    public void PlaySFXOnceNoInterrupt(GameSounds sound)
    {
        if (sfxAudioSource.isPlaying) return;
        sfxAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    public void PlayRandomFootstepConcrete()
    {
        sfxAudioSource.PlayOneShot(GetRandomAudioClip());
    }

    public void StopAndPlaySFXOnce(AudioClip audioClip)
    {
        sfxAudioSource.Stop();
        sfxAudioSource.PlayOneShot(audioClip);
    }

    public void PlayMusicLoop(AudioClip audioClip)
    {
        musicAudioSource.loop = true;
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }

    public void StopAll()
    {
        musicAudioSource.Stop();
        sfxAudioSource.Stop();
    }

    public void PlayMusicOnceNoInterrupt(GameSounds sound)
    {
        if (musicAudioSource.isPlaying) return;
        musicAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    public void SetMasterVolume(float sliderValue)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }
    public void SetSFXVolume(float sliderValue)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    private AudioClip GetAudioClip(GameSounds sound)
    {
        foreach (SoundAudioClip audioClip in audioClipArray)
        {
            if (audioClip.sound == sound)
            {
                return audioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }

    private AudioClip GetRandomAudioClip()
    {
        int rand = Random.Range(0, footstepClipArray.Length);
        return footstepClipArray[rand];
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.GameSounds sound;
        public AudioClip audioClip;
    }
}
