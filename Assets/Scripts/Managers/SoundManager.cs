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
    public MusicAudioClip[] musicClipArray;
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
        MenuMasterSlider,
        PlayerPistolShoot,
        WinSound,
        GrappleShoot,
        GrappleImpact
    }

    public enum MusicTracks
    {
        MainMenu,
        GameplayDNB,
        GameplayBass,
        Victory,
        Death
    }

    public void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
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

    public void PlaySFXOnce(AudioClip sound)
    {
        sfxAudioSource.PlayOneShot(sound);
    }

    public void PlaySFXOnceNoInterrupt(GameSounds sound)
    {
        if (sfxAudioSource.isPlaying) return;
        sfxAudioSource.PlayOneShot(GetAudioClip(sound));
    }

    public void PlayRandomFootstepConcrete()
    {
        sfxAudioSource.PlayOneShot(GetRandomAudioClip(footstepClipArray));
    }

    public void StopAndPlaySFXOnce(AudioClip audioClip)
    {
        sfxAudioSource.Stop();
        sfxAudioSource.PlayOneShot(audioClip);
    }

    public void PlayMusicLoop(MusicTracks audioClip, bool pickRandomGameplayTrack = false)
    {
        musicAudioSource.Stop();
        musicAudioSource.loop = true;
        if (pickRandomGameplayTrack)
            musicAudioSource.clip = GetRandomMusicAudioClip();
        else
            musicAudioSource.clip = GetMusicAudioClip(audioClip);
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

    private AudioClip GetMusicAudioClip(MusicTracks sound)
    {
        foreach (MusicAudioClip audioClip in musicClipArray)
        {
            if (audioClip.sound == sound)
            {
                return audioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }

    private AudioClip GetRandomMusicAudioClip()
    {
        int rand = Random.Range(1, musicClipArray.Length - 2);
        return musicClipArray[rand].audioClip;
    }

    private AudioClip GetRandomAudioClip(AudioClip[] audioClips)
    {
        int rand = Random.Range(0, audioClips.Length);
        return audioClips[rand];
    }

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.GameSounds sound;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class MusicAudioClip
    {
        public SoundManager.MusicTracks sound;
        public AudioClip audioClip;
    }
}
