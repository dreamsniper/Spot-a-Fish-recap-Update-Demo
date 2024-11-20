using System.Collections;
using UnityEngine;

public class RegularBubbleRising : MonoBehaviour
{
    public GameObject bubblePrefab; 
    public float riseSpeed = 1.0f; 
    public float spawnInterval = 1.0f; 

    void Start()
    {
        // Start spawning bubbles
        StartCoroutine(SpawnBubbles());
    }

    IEnumerator SpawnBubbles()
    {
        while (true)
        {
            // new bubble at the current position
            GameObject bubble = Instantiate(bubblePrefab, transform.position, Quaternion.identity);
            
            // move the bubble upwards
            StartCoroutine(MoveBubbleUpwards(bubble));

            // Wait for the next spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveBubbleUpwards(GameObject bubble)
    {
        // Move the bubble upwards until it's destroyed
        while (bubble != null)
        {
            bubble.transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
