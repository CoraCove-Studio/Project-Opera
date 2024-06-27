using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager: MonoBehaviour
{
    [SerializeField] public Sound[] musicSounds, sfxSounds;
    [SerializeField] public AudioSource musicSource, sfxSource;

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

    private void Start()
    {
        PlayMusic("Theme");
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log( name + " audio file could not be found.");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

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

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }


}
