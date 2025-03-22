using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    public List<AudioClip> audioClips;

    [Range(0f, 1f)]
    public float pitchRandomness = 0.1f; // Range of pitch randomness (e.g., 0.1 means pitch can vary by ±0.1)

    [Range(0f, 1f)]
    public float volumeRandomness = 0.2f; // Range of volume randomness (e.g., 0.2 means volume can vary by ±0.2)

    public bool avoidRepeatingLast = true; // If true, avoid playing the last played audio clip
    public bool playOnAwake = false;

    [SerializeField]
    private AudioSource audioSource;

    private AudioClip lastPlayedClip;

    private float startingVolume;
    private float startingPitch;


    private void Awake()
    {
        startingVolume = audioSource.volume;
        startingPitch = audioSource.pitch;
        if (playOnAwake) PlayRandomAudio();
    }

    public void PlayRandomAudio()
    {

        if (audioClips.Count == 0)
        {
            return;
        }

        // Pick a random clip, ensuring it isn't the same as the last played one if the flag is set
        AudioClip selectedClip;
        do
        {
            selectedClip = audioClips[Random.Range(0, audioClips.Count)];
        } while (avoidRepeatingLast && selectedClip == lastPlayedClip);

        audioSource.pitch = startingPitch + Random.Range(-pitchRandomness, pitchRandomness); // Random pitch between 1-pitchRandomness and 1+pitchRandomness
        audioSource.volume = startingVolume + Random.Range(-volumeRandomness, volumeRandomness); // Random volume between 1-volumeRandomness and 1+volumeRandomness

        audioSource.PlayOneShot(selectedClip);

        lastPlayedClip = selectedClip;
    }
}