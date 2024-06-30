using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
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

    private void Awake()
    {

        // Initialize and shuffle availableBeats list from beats array
        // availableBeats = new List<AudioClip>(beats);
        // ShuffleBeats();
        //if (beats.Length > 0)
        //{
        //    beatDuration = beats[0].length; // Assuming all beats have the same length
        //}
        // nextStartTime = AudioSettings.dspTime; // Initialize the next start time
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