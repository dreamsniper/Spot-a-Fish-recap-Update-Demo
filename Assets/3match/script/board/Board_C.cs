using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public partial class Board_C : MonoBehaviour {


    public static GameObject this_board; //used by ChargeBonusButton.cs
    [HideInInspector] public bool boardGenerated = false;


    public Transform cursor;

    bool diagonal_falling_preference_direction_R; //it is alternate in each step


    //interaction
    [HideInInspector] public int touch_number;
    [HideInInspector] public bool player_can_move;
    [HideInInspector] public int n_combo;//number of combo explosion after main explosion
    [HideInInspector] public bool waitForDelay;





    #region visual fx
    [HideInInspector]
	public bool activate_main_gem_fx_for_big_explosion;
    [HideInInspector]
    public bool activate_minor_gem_fx_for_big_explosion;


	#endregion



    [HideInInspector] public bool player_win;

	#region rules

	//public bool continue_to_play_after_win_until_lose_happen;
    [HideInInspector] public int total_gems_on_board_at_start;//need for destroy_all_gems
    [HideInInspector] public bool all_explosions_are_completed;




    //manage turns
    //public bool versus = false;//true = play versus AI
    [HideInInspector] public bool player_turn = false;//keep it false here in order to give the first move to player


    [HideInInspector] public bool game_end;











    [HideInInspector] public int number_of_junk_on_board;

    [HideInInspector] public int number_of_token_on_board;
    [HideInInspector] public int number_of_token_to_collect;
    [HideInInspector] public int number_of_token_collected;

        [HideInInspector] public bool[,]token_place_card;//if "show_token_after_all_tiles_are_destroyed" is true, use this to keep track of tokens positions
		bool token_showed;



	bool turn_gained;


    #endregion



    //manage update board after the move

    [HideInInspector] public int number_of_gems_to_move;
    [HideInInspector] public int number_of_new_gems_to_create;
    [HideInInspector] public int number_of_gems_to_mix;
    [HideInInspector] public bool gem_falling_ongoing;


    [HideInInspector] public int HP_board;//if 0 mean that all tiles are destroyed (victory requirement)
	//public GameObject tile_obj;
    //public Sprite[] tile_hp; //the avatar of the tile


    [HideInInspector] public List<tile_C> reserved_for_primary_explosion;



    public void InitiateGame() {


        if (myRuleset.start_after_selected == RulesetTemplate.start_after.time)
			{
            if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
                {
                if (myRuleset.start_after_n_seconds <= 0)
                    StartBoardUpdate();
                else
                    Invoke("StartBoardUpdate", myRuleset.start_after_n_seconds);
            }
            else
                {
                if (myRuleset.start_after_n_seconds <= 0)
                    Check_ALL_possible_moves();
                else
                    Invoke("Check_ALL_possible_moves", myRuleset.start_after_n_seconds);
                }
            }
		else//show info_screen
			{
            uIManager.infoScreenText.text = myRuleset.start_info_screen_text;
            uIManager.gui_info_screen.SetActive(true);
			}



    }

	public void My_start()//call from Canvas > info_screen > button
	{
        uIManager.gui_info_screen.SetActive(false);

        if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
            StartBoardUpdate();
        else
		    Check_ALL_possible_moves();

    }


    void Update()
	{
        
        if (!stage_started)
            return;

        //debug
        /*
        for (int i = 0; i < tileContents.Count; i++)
        {
            if (tileContents[i].ImOrphan())
            {
                Debug.LogError("ORPHAN GEM!");
                return;
            }

        }
        */
        //


        uIManager.Slider_hp_animation(player);
        uIManager.Slider_hp_animation(enemy);

        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.timer)
			Timer();

 
        FallingManager();

        if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
            BoardUpdate();


    }

    [HideInInspector] public List<tile_C> fallingTiles;
    void FallingManager()
    {
        if (fallingTiles.Count < 1)
            return;

        for (int i = fallingTiles.Count-1; i >= 0; i--)
        {
            fallingTiles[i].Falling();
        }
    }


}
