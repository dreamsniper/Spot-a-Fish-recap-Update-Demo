using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = Vector2.zero;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Cutscene-1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("W1_Stage_1");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ConservationTank()
    {
        SceneManager.LoadScene("Conservation-Tank");
    }

      public void TimeTrialButton()
    {
        SceneManager.LoadScene("Time-Trial");
    }

/*    public void Open()
    {
        transform.LeanScale(Vector2.one, 0.8f);
    }

    public void Close()
    {
        transform.LeanScale(Vector2.zero, 1f).setEaseInBack();
    }*/
}
