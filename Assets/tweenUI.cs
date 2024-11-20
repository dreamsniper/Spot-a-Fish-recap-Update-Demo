using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenUI : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject sunburtsBg;
    [SerializeField] private GameObject logoTitle;


    // Start is called before the first frame update
    void Start()
    {
        // Rotate sunburtsBg around the Z-axis (forward) continuously
        LeanTween.rotateAround(sunburtsBg, Vector3.forward, -360, 15f).setDelay(0.5f).setLoopClamp();
        
        // Scale logoTitle to 60% of its original size with an elastic easing effect
        LeanTween.scale(logoTitle, new Vector3(0.60f, 0.60f, 0.60f), 2f).setDelay(0.5f).setEase(LeanTweenType.easeOutElastic);
        
        // Scale optionsPanel to 60% of its original size with an elastic easing effect
        LeanTween.scale(optionsPanel, new Vector3(0.60f, 0.60f, 0.60f), 2f).setDelay(0.5f).setEase(LeanTweenType.easeOutElastic);
    }

    // Method to open the UI panel by scaling it to its full size
    public void Open()
    {
        transform.LeanScale(Vector3.one, 0.8f); // Use Vector3 for consistency
    }

    // Method to close the UI panel by scaling it down to zero
    public void Close()
    {
        transform.LeanScale(Vector3.zero, 1f).setEaseInBack(); // Use Vector3 for consistency
    }
}
