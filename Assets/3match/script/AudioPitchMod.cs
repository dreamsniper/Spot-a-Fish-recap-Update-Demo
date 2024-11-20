using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPitchMod : MonoBehaviour
{
    public AudioSource audioSource;     
    public AudioClip targetClip;               
    private float originalPitch = .63f;  

    void Start()
    {
        // Set the initial pitch
        originalPitch = audioSource.pitch;
    }

    void Update()
    {
        // Check if the target clip is currently playing
        if (audioSource.isPlaying && audioSource.clip == targetClip)
        {

            float pitchChange = Random.Range(-1f, 1f);  // Randomize pitch between -1 and +1
            audioSource.pitch = Mathf.Clamp(originalPitch + pitchChange, 0.5f, 2f); // Set pitch with limits
        }
        else
        {
            // Reset the pitch when the clip is not playing
            audioSource.pitch = originalPitch;
        }
    }
}
