using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    // ATTENTION
    // when building sliders, min needs to be = 0.0001 and max is 1

    [SerializeField] private AudioMixer masterMixer;

    public void SetMasterVolume(float volumeLevel)
    {
        masterMixer.SetFloat("masterVolume", Mathf.Log10(volumeLevel) * 20f);
    }

    public void SetSFXVolume(float volumeLevel)
    {
        masterMixer.SetFloat("sfxVolume", Mathf.Log10(volumeLevel) * 20f);
    }

    public void SetMusicVolume(float volumeLevel)
    {
        masterMixer.SetFloat("musicVolume", Mathf.Log10(volumeLevel) * 20f);
    }
}
