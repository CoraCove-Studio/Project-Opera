using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sfxSounds, ambienceSounds;
    [SerializeField] private AudioClip[] beats;
    [SerializeField] private List<AudioSource> sfxSources;
    [SerializeField] private AudioSource ambienceSource;

    [SerializeField] private AudioClip errorNoise;
    [SerializeField] private AudioClip notificationNoise;
    [SerializeField] private AudioClip debtPaymentNoise;

    [SerializeField] private List<AudioClip> inputNoises;

    [Header("Audio Mixers")]
    public AudioMixer masterMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    private List<AudioClip> availableBeats;
    //private double nextStartTime;
    //private float beatDuration;

    public void PlayErrorNoise()
    {
        PlaySFX(errorNoise);
    }

    public void PlayNotificationNoise()
    {
        PlaySFX(notificationNoise);
    }

    public void PlayDebtPaymentNoise()
    {
        PlaySFX(debtPaymentNoise);
    }

    public AudioClip ReturnRandomInputNoise()
    {
        return inputNoises[Random.Range(0, inputNoises.Count)];
    }

    public void SetAudioMixerValues(float musicVol, float sfxVol)
    {
        masterMixer.SetFloat("musicVolume", musicVol);
        masterMixer.SetFloat("sfxVolume", sfxVol);
    }

    private void Awake()
    {
        InitializeMixerValues();
    }

    private void InitializeMixerValues()
    {
        GameManager gm = GameManager.Instance;
        SetAudioMixerValues((float)gm.SettingsDictionary["MusicVolume"], (float)gm.SettingsDictionary["SFXVolume"]);
    }

    public void PlaySFX(AudioClip clip)
    {
        foreach (var source in sfxSources)
        {
            if (source.isPlaying == false)
            {
                source.PlayOneShot(clip);
                return;
            }
        }
    }
}