using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StageLoader : MonoBehaviour
{
    private BoardManager boardManager;
    public string stageName;          // Name of the stage to load
    public Camera cam;                // Camera reference
    public float camPosX = 4f;        // Default X position for the camera
    public float camPosY = -4.5f;     // Default Y position for the camera
    public Button restartButton;      // Restart button reference

    void Start()
    {
        // Initialize BoardManager
        boardManager = GetComponent<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager not found on GameObject.");
            return;
        }
        
        // Load the specified stage
        boardManager.LoadStage(stageName);

        // Find and reposition the camera
        cam = cam ?? Camera.main;
        if (cam != null)
        {
            RepositionCamera();
        }
        else
        {
            Debug.LogError("Camera not found in the scene.");
        }

        // Add listener to restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonPressed);
        }
        else
        {
            Debug.LogWarning("Restart button not assigned in the Inspector.");
        }
    }

    /// Repositions the camera to the specified coordinates.
    public void RepositionCamera()
    {
        if (cam != null)
        {
            cam.transform.position = new Vector3(camPosX, camPosY, cam.transform.position.z);
        }
    }

    /// Handles restart button press, repositions the camera, and resets the stage.
    public void OnRestartButtonPressed()
    {
        Debug.Log("Restart button pressed.");
        RepositionCamera();
        boardManager.LoadStage(stageName);  // Optionally reload the stage
    }
}
