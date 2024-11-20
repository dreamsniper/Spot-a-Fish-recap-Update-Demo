using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowEffect : MonoBehaviour
{
    public string tokenName = "token"; // Name of the token in the hierarchy
    public Material glowingSpriteMaterial; // Reference to the glowing material

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApplyGlowEffect());
    }

    IEnumerator ApplyGlowEffect()
    {
        // Wait until the token is created
        yield return new WaitUntil(() => GameObject.Find(tokenName) != null);

        // Find the token in the hierarchy
        GameObject token = GameObject.Find(tokenName);

        if (token != null)
        {
            // Get the SpriteRenderer component of the token
            SpriteRenderer tokenSpriteRenderer = token.GetComponentInChildren<SpriteRenderer>();

            if (tokenSpriteRenderer != null)
            {
                // Apply the glowing material
                tokenSpriteRenderer.material = glowingSpriteMaterial;
                Debug.Log("glowing");
            }
            else
            {
                Debug.LogError("SpriteRenderer component not found on the token!");
            }
        }
        else
        {
            Debug.LogError("Token not found in the hierarchy!");
        }
    }
}
