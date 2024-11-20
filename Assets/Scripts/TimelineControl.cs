using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineControl : MonoBehaviour
{
    public PlayableDirector playableDirector;  // Reference to the Playable Director controlling the timeline
    public Button continueButton;  // Reference to the UI button

    private bool isWaitingForInput = false;  // Track if waiting for input

    void Start()
    {
        // Ensure continueButton is assigned and hide it initially
        if (continueButton == null)
        {
            Debug.LogError("Continue Button is not assigned in the Inspector!");
            return;
        }

        continueButton.gameObject.SetActive(false);
        continueButton.onClick.AddListener(ResumeTimeline);  // Assign listener for button click
    }

    // Pauses the timeline
    public void PauseTimeline()
    {
        // Ensure PlayableDirector is assigned
        if (playableDirector == null)
        {
            Debug.LogError("PlayableDirector is not assigned in the Inspector!");
            return;
        }

        Debug.Log("Timeline Paused");  // Debug to verify if pause is triggered
        playableDirector.Pause();  // Pause the timeline
        isWaitingForInput = true;
        continueButton.gameObject.SetActive(true);  // Show the continue button
    }

    // Resumes timeline
    public void ResumeTimeline()
    {
        if (isWaitingForInput && playableDirector != null)
        {
            Debug.Log("Resuming Timeline");  // Debug to verify if resume is triggered
            playableDirector.Play();  // Resume the timeline
            isWaitingForInput = false;
            continueButton.gameObject.SetActive(false);  // Hide the continue button
        }
    }
}
