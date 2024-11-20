using System.Collections;
using UnityEngine;

public class BubbleSpawn : MonoBehaviour
{
    public GameObject bubblePrefab; // bubble prefab (incase it cold be animated)
    public float riseSpeed = 1.0f; // bubbles rising speed
    public float spawnInterval = 1.0f; // Time interval between bubble spawns

    void Start()
    {
        // Start spawning bubbles coroutine spawns it after certain time
        StartCoroutine(SpawnBubbles());
    }

    IEnumerator SpawnBubbles()
{
    while (true)
    {
        // Create a new bubble at the current position
        // Set z position to ensure it's in front of the UI if necessary
        GameObject bubble = Instantiate(bubblePrefab, transform.position + new Vector3(0, 0, -1), Quaternion.identity);

        // Start moving the bubble upwards
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
