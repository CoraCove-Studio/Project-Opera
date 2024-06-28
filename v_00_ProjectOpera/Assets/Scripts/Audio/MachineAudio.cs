using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MachineAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip assignedBeat;

    private void Awake()
    {
        // Add and configure the AudioSource component
        audioSource = GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = AudioManager.Instance.musicGroup;
        audioSource.loop = true;
    }

    private void Start()
    {
        AssignUniqueBeat();
    }

    private void AssignUniqueBeat()
    {
        assignedBeat = AudioManager.Instance.GetUniqueBeat();
        if (assignedBeat != null)
        {
            audioSource.clip = assignedBeat;
            SyncWithGlobalTimer();
        }
    }

    private void SyncWithGlobalTimer()
    {
        double nextStartTime = AudioManager.Instance.GetNextStartTime();
        audioSource.PlayScheduled(nextStartTime);
    }

    private void OnDestroy()
    {
        if (assignedBeat != null)
        {
            AudioManager.Instance.ReturnBeat(assignedBeat);
        }
    }
}
