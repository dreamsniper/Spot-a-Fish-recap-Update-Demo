using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioClip destructionSound; // Sound to play on destruction
    private AudioSource audioSource;

    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>(); // Find an AudioSource in the scene
    }

    void OnDestroy()
    {
        if (audioSource != null && destructionSound != null)
        {
            audioSource.PlayOneShot(destructionSound);
            Debug.Log($"{gameObject.name} has been destroyed.");
        }
    }
}
