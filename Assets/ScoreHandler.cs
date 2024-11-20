using UnityEngine;
using UnityEngine.UI; 

public class ScoreHandler : MonoBehaviour
{
    public Text scoreText; 
    public GameObject[] spritesToDisappear;
    private int currentScore = 0; 

    void Start()
    {
        scoreText.text = currentScore.ToString();
    }

    void Update()
    {
        int score;
        if (int.TryParse(scoreText.text, out score))
        {
            if (score > currentScore && score <= spritesToDisappear.Length)
            {
                currentScore = score;

                // Make the sprite corresponding to the current score disappear
                spritesToDisappear[currentScore - 1].SetActive(false);  // currentScore - 1 to access the array properly
                Debug.Log("dissapear");
            }
        }
    }
}
