using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

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

    public void PlaySFXOnce(AudioClip audioClip)
    {
        sfxAudioSource.PlayOneShot(audioClip);
    }

    public void PlaySFXOnceNoInterrupt(AudioClip audioClip)
    {
        if (sfxAudioSource.isPlaying) return;
        sfxAudioSource.PlayOneShot(audioClip);
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

    public void PlayMusicOnceNoInterrupt(AudioClip audioClip)
    {
        if (musicAudioSource.isPlaying) return;
        musicAudioSource.PlayOneShot(audioClip);
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
}
