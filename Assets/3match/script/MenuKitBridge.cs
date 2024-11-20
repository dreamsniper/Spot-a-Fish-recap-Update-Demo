using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuKitBridge : MonoBehaviour {

    
    public GameObject Stage_uGUI_obj;

    /*
    game_master my_game_master;
    game_uGUI my_game_uGUI;
    */
    BoardManager myBoardManager;

    private void Start()
    {
        StartMe();
    }

    public void StartMe()
    {
        /*
        if (game_master.game_master_obj)
            my_game_master = (game_master)game_master.game_master_obj.GetComponent("game_master");

        my_game_uGUI = (game_uGUI)Stage_uGUI_obj.GetComponent("game_uGUI");
        myBoardManager = GetComponent<BoardManager>();

        LoadStage();
        */
    }

    private void LateUpdate()
    {
        /*
        if (my_game_uGUI && my_game_uGUI.restarting)
        {
            LoadStage();
            my_game_uGUI.restarting = false;
        }
        */
    }

    void LoadStage()
    {
        /*
        string stage = "W" + my_game_uGUI.n_world + "_Stage_" + my_game_uGUI.n_stage;
        print("MenuKitBridge load: " + stage);
        myBoardManager.LoadStage(stage);
        */
    }

    public void WinScreen_menuKit(int finalStarScore)
    {
        /*
        my_game_uGUI.star_number = finalStarScore;
        Debug.Log(my_game_uGUI.star_number + " stars");
        my_game_uGUI.Victory();
        */
    }

    public void UpdateScore(int playerScore)
    {
        /*
        my_game_uGUI.int_score = playerScore;
        my_game_uGUI.Update_int_score();
        */
    }

    public void LoseScreen_menuKit()
    {
        /*
        my_game_uGUI.Defeat();
        */
    }

    public void SfxMenuKit(AudioClip my_clip)
    {
        /*
        if (my_clip != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Stop(); // Stop any currently playing sound
                    audioSource.clip = my_clip;
                    audioSource.Play(); // Play the new sound
                }
                else
                {
                    Debug.LogError("No AudioSource component found on the GameObject.");
                }
        }*/
    }

}
