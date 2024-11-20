using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Board_C : MonoBehaviour
{

    
    public void Reset_variables()//call from Initiate_variables()
    {
        print("Reset_variables");
        uIManager.ResetUI();
        Cancell_hint();
        ResetTimer();
        HP_board = 0;
        block_count = 0;
        primary_explosion_count = 0;
        number_of_elements_to_damage = 0;
        number_of_elements_to_damage_with_SwitchingGems = 0;
        number_of_elements_to_damage_with_bonus = 0;
        number_of_gems_to_move = 0;
        number_of_new_gems_to_create = 0;
        stage_started = false;
        player_win = false;
        game_end = false;
        game_is_ended = false;//this avoid to call end screen multiple times when player_can_move_when_gem_falling = true;
        player_turn = false;
        shuffleAttemptsCount = 0;
        current_board_orientation = 0;
        number_of_token_collected = 0;
        number_of_token_emitted = 0;
        number_of_token_to_collect = 0;
        number_of_token_on_board = 0;
        token_showed = false;
        number_of_tiles_leader = 0;
        elements_to_damage_list = new List<tile_C>();
        fallingTiles = new List<tile_C>();
        current_turn = 0;

        iceGrowturnCount = 0;
        iceGrowTargetTime = 0;


    }

    public void Initiate_variables()//call from Awake
    {
        Reset_variables();
        this_board = this.gameObject;

        cursor.gameObject.SetActive(false);


        //emission rules
        Initiate_emitter_variables();


        uIManager.playerUI.gui_bonus_ico_array = new GameObject[myRuleset.gem_length];
        uIManager.enemyUI.gui_bonus_ico_array = new GameObject[myRuleset.gem_length];


        current_star_score = 0;


        if (myRuleset.versus)
            {
            enemy_AI_preference_order = new enemy_AI_manual_setup[myRuleset.gem_length];
            for (int n = 0; n < myRuleset.gem_length; n++)
                enemy_AI_preference_order[n] = enemy.myCharacter.temp_enemy_AI_preference_order[n];
            }


        if (myRuleset.use_armor)
            Find_gem_damage_opponent_max_value();

            

        time_left = myRuleset.timer;



        uIManager.Initiate_ugui();
    }

}
