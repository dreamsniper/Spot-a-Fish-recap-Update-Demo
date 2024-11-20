using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIBubbleRising : MonoBehaviour
{
    public GameObject bubblePrefab; // The bubble prefab to instantiate (should be a UI Image)
    public float riseSpeed = 100.0f; // Speed at which the bubbles rise
    public float spawnInterval = 1.0f; // Time interval between bubble spawns
    public RectTransform canvasRect; // Reference to the Canvas RectTransform

    public float minSize = 0.5f; // Minimum size of the bubble
    public float maxSize = 1.5f; // Maximum size of the bubble

    // Start is called before the first frame update
    void Start()
    {
        // Start spawning bubbles
        StartCoroutine(SpawnBubbles());
    }

    IEnumerator SpawnBubbles()
    {
        while (true)
        {
            // Create a new bubble
            GameObject bubble = Instantiate(bubblePrefab, transform);

            // Set a random horizontal position within the canvas width
            float randomX = Random.Range(-400, canvasRect.rect.width);
            bubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(randomX, -200);

            // Set a random size
            float randomSize = Random.Range(minSize, maxSize);
            bubble.GetComponent<RectTransform>().localScale = new Vector3(randomSize, randomSize, 1);

            // Start moving the bubble upwards
            StartCoroutine(MoveBubbleUpwards(bubble));

            // Wait for the next spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveBubbleUpwards(GameObject bubble)
    {
        RectTransform bubbleRect = bubble.GetComponent<RectTransform>();

        // Move the bubble upwards until it moves off the top of the canvas
        while (bubble != null && bubbleRect.anchoredPosition.y < canvasRect.rect.height)
        {
            bubbleRect.anchoredPosition += new Vector2(0, riseSpeed * Time.deltaTime);
            yield return null;
        }

        // Destroy the bubble after it moves off the screen
        if (bubble != null)
        {
            Destroy(bubble);
        }
    }
}
