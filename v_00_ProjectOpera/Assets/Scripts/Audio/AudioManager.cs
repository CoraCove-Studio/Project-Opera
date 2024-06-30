using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager: MonoBehaviour
{
    [SerializeField] private Sound[] sfxSounds, ambienceSounds;
    [SerializeField] private AudioClip[] beats;
    [SerializeField] private AudioSource sfxSource, ambienceSource;

    [Header("Audio Mixers")]
    public AudioMixer masterMixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    private List<AudioClip> availableBeats;
    //private double nextStartTime;
    //private float beatDuration;



    private static bool isQuitting = false;
    private static AudioManager instance;
    public static GameObject audioManagerPrefab;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null && isQuitting != true)
            {
                if (audioManagerPrefab == null)
                {
                    Debug.LogError("AudioManager prefab is not assigned.");
                    return null;
                }

                GameObject audioManager = Instantiate(audioManagerPrefab);
                
                if (!audioManager.TryGetComponent(out instance))
                {
                    Debug.LogError("The AudioManager prefab does not have an AudioManager component.");
                    return null;
                }

                DontDestroyOnLoad(audioManager);
                Debug.Log("New AudioManager has been created.");
            }
            return instance;
        }
    }

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

    //public AudioClip ReturnRandomMachineProductionClip()
    //{
    //    Debug.Log("AudioManager: ReturnRandomMachineProductionClip: Returning clip.");
    //    Debug.Log($"machineProductionLoops list contains {machineProductionLoops.Count} elements.");
    //    return machineProductionLoops[Random.Range(0, machineProductionLoops.Count - 1)];
    //}


    #region Rachel's Methods
    //private void Update()
    //{
    //    // Continuously update the next start time to maintain synchronization
    //    if (AudioSettings.dspTime >= nextStartTime)
    //    {
    //        nextStartTime += beatDuration;
    //    }
    //}
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