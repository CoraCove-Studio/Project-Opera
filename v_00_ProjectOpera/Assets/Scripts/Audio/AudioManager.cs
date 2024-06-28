using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager: MonoBehaviour
{
    [SerializeField] public Sound[] sfxSounds, ambienceSounds;
    [SerializeField] public AudioClip[] beats;
    [SerializeField] public AudioSource sfxSource, ambienceSource;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("Audio Manager is NULL!");
            }
            return instance;
        }
    }

    [Header("Audio Mixers")]
    public AudioMixer masterMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    private List<AudioClip> availableBeats;
    private double nextStartTime;
    private float beatDuration;

    private void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize availableBeats list from beats array
            availableBeats = new List<AudioClip>(beats);
            if (beats.Length > 0)
            {
                beatDuration = beats[0].length; // Assuming all beats have the same length
            }
            nextStartTime = AudioSettings.dspTime; // Initialize the next start time
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        // Continuously update the next start time to maintain synchronization
        if (AudioSettings.dspTime >= nextStartTime)
        {
            nextStartTime += beatDuration;
        }
    }

    public AudioClip GetUniqueBeat()
    {
        if (availableBeats.Count == 0)
        {
            Debug.LogError("No available beats!");
            return null;
        }
        AudioClip beat = availableBeats[0];
        availableBeats.RemoveAt(0);
        return beat;
    }

    public void ReturnBeat(AudioClip beat)
    {
        availableBeats.Add(beat);
    }

    public double GetNextStartTime()
    {
        return nextStartTime;
    }

    public float GetBeatDuration(AudioClip beat)
    {
        return beat.length;
    }

    //public void PlayMusic(string name)
    //{
    //    Sound s = Array.Find(musicSounds, x => x.name == name);

    //    if (s == null)
    //    {
    //        Debug.Log( name + " audio file could not be found.");
    //    }
    //    else
    //    {
    //        musicSource.clip = s.clip;
    //        musicSource.Play();
    //    }
    //}

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log(name + " audio file could not be found.");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    //public void StopMusic()
    //{
    //    musicSource.Stop();
    //    musicSource.clip = null;
    //}
}