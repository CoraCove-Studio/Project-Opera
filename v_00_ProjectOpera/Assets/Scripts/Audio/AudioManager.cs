using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager: MonoBehaviour
{
    [SerializeField] private Sound[] sfxSounds, ambienceSounds;
    [SerializeField] private AudioClip[] beats;
    [SerializeField] private AudioSource sfxSource, ambienceSource;

    private static bool isQuitting = false;
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null && isQuitting != true)
            {
                GameObject audioManager = new("AudioManager");
                instance = audioManager.AddComponent<AudioManager>();
                DontDestroyOnLoad(audioManager);
                Debug.Log("New AudioManager has been created.");
            }
            return instance;
        }
    }

    private void OnEnable()
    {
        Application.quitting += Quitting;
    }

    private void OnDisable()
    {
        Application.quitting -= Quitting;
    }

    private void Quitting()
    {
        isQuitting = true;
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

            // Initialize and shuffle availableBeats list from beats array
            // availableBeats = new List<AudioClip>(beats);
            // ShuffleBeats();
            //if (beats.Length > 0)
            //{
            //    beatDuration = beats[0].length; // Assuming all beats have the same length
            //}
            // nextStartTime = AudioSettings.dspTime; // Initialize the next start time
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

    #region Rachel's Methods
    //public AudioClip GetUniqueBeat()
    //{
    //    if (availableBeats.Count == 0)
    //    {
    //        Debug.LogError("No available beats!");
    //        return null;
    //    }
    //    AudioClip beat = availableBeats[0];
    //    availableBeats.RemoveAt(0);
    //    return beat;
    //}

    //public void ReturnBeat(AudioClip beat)
    //{
    //    availableBeats.Add(beat);
    //}

    //public double GetNextStartTime()
    //{
    //    return nextStartTime;
    //}

    //public float GetBeatDuration(AudioClip beat)
    //{
    //    return beat.length;
    //}

    //private void ShuffleBeats()
    //{
    //    for (int i = 0; i < availableBeats.Count; i++)
    //    {
    //        AudioClip temp = availableBeats[i];
    //        int randomIndex = UnityEngine.Random.Range(i, availableBeats.Count);
    //        availableBeats[i] = availableBeats[randomIndex];
    //        availableBeats[randomIndex] = temp;
    //    }
    //}

    //public void PlaySFX(string name)
    //{
    //    Sound s = Array.Find(sfxSounds, x => x.name == name);

    //    if (s == null)
    //    {
    //        Debug.Log(name + " audio file could not be found.");
    //    }
    //    else
    //    {
    //        sfxSource.PlayOneShot(s.clip);
    //    }
    //}

    #endregion
}