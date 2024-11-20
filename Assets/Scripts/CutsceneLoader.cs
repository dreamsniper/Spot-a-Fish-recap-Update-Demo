using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneLoader : MonoBehaviour
{

    public Image[] cutsceneImages;
    public float displayTime = 2.0f;  // Time each image is displayed

    // Start is called before the first frame update
    void Start()
    {
        if (cutsceneImages.Length != 3)
        {
            Debug.LogError("assign 3 images to the Cutscene.");
            return;
        }

        // Start the cutscene sequence
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        foreach (Image image in cutsceneImages)
        {
            // Enable the image
            image.gameObject.SetActive(true);

            // Wait for the display time
            yield return new WaitForSeconds(displayTime);

            // Disable the image
            image.gameObject.SetActive(false);
        }
    }
}