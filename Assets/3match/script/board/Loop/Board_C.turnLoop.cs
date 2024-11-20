using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour {

    [SerializeField] bool debugAutoplay;
    int current_turn = 0;
    void UpdateTurn()//call from Check_ALL_possible_moves(), Update_turn_order_after_a_bad_move(), Player_lose()
    {
        print("UpdateTurn()");

        //Debug.Log("number_of_bonus_on_board: " + number_of_bonus_on_board);
        //reset variables of the previous move
        n_combo = 0;
        number_of_padlocks_involved_in_explosion = 0;
        number_of_elements_to_damage = 0;
        number_of_elements_to_damage_with_bonus = 0;
        number_of_elements_to_damage_with_SwitchingGems = 0;
        number_of_gems_to_move = 0;
        number_of_new_gems_to_create = 0;
        gems_useful_moved = 0;
        bonus_select = Bonus.None;
        all_explosions_are_completed = false;

        CheckLose_player_have_zero_moves();


        if (!stage_started)
        {
            start_time = Time.timeSinceLevelLoad;
            stage_started = true;
        }
        if (!game_end)
        {
            //ListOfPotentialMoves();

            current_turn++;

            temp_direction = -1;

            Debug.Log("********** turn " + current_turn);
            UpdateTurnBasedIceLoop();

            current_moveStatus = Board_C.moveStatus.waitingNewMove;
            if (!myRuleset.versus)//if player play alone
            {
                if (turn_gained)
                {
                    turn_gained = false;
                    player.myCharacter.currentChainLenght++;
                }
                else
                    player.myCharacter.currentChainLenght = 0;


                player_turn = true;
                player_can_move = true;

                if (globalRules.show_hint)
                {
                    use_hint = true;
                    Invoke("Show_hint", globalRules.show_hint_after_n_seconds);
                }
            }
            else //if player play against AI
            {
                if (turn_gained)
                {
                    turn_gained = false;
                    activeCharacter.myCharacter.currentChainLenght++;

                }
                else
                {
                    if (player_turn)//if the previous was of the player
                    {
                        player_turn = false;// now is enemy turn
                        player.myCharacter.currentChainLenght = 0;
                        activeCharacter = enemy;
                        passiveCharacter = player;
                    }
                    else
                    {
                        player_turn = true;//else is palyer turn
                        enemy.myCharacter.currentChainLenght = 0;
                        activeCharacter = player;
                        passiveCharacter = enemy;
                    }
                }

                if (player_turn)
                {
                    Debug.Log("Player's turn");
                    player_can_move = true;
                    player.myUI.gui_name_text.color = player.myUI.gui_color_on;
                    enemy.myUI.gui_name_text.color = enemy.myUI.gui_color_off;

                    if (globalRules.show_hint)
                    {
                        use_hint = true;
                        Invoke("Show_hint", globalRules.show_hint_after_n_seconds);
                    }


                    if (debugAutoplay)
                        Enemy_play();
                }
                else
                {
                    player.myUI.gui_name_text.color = player.myUI.gui_color_off;
                    enemy.myUI.gui_name_text.color = enemy.myUI.gui_color_on;
                    Enemy_play();
                }
            }
        }
        else
        {
            Game_end();
        }
    }




    
    public void Start_update_board()//call from: tile_C.update.If_all_explosion_are_completed(), tile_C.update.Check_if_gem_movements_are_all_done()
    {
        Debug.Log("Start_update_board");

        //if (!player_can_move_when_gem_falling)
        cursor.gameObject.SetActive(false);

        if (myRuleset.diagonal_falling)
        {
            Debug.Log("diagonal_falling");
            Check_new_gem_to_generate();

            Check_vertical_falling();
            Check_diagonal_falling();

            //now you know what gems must be fall, so...
            Update_board_by_one_step();

        }
        else //diagonal falling not allowed
        {
            Check_new_gem_to_generate();
            Check_vertical_falling();
            //now you know what gems must be fall, so...
            Update_board_by_one_step();


            /* OLD METHOD:
            //print("diagonal falling not allowed");
            //read board form down to up (and left to right)
            for (int y = _Y_tiles - 1; y >= 0; y--)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if ((board_array_master[x, y, 0] > -1) && (board_array_master[x, y, 13] == 0))//if there is a tile unchecked...
                    {
                        //...and it is empty
                        if (board_array_master[x, y, 1] == -99)
                        {
                            script_tiles_array[x,y].Count_how_much_gem_there_are_to_move_over_me();
                        }
                    }
                }
            }
            //now you know what gems must be fall, so...
            Start_gem_fall();
            */
        }

    }
    
    void Update_board_by_one_step()
    {
        //Debug.Log("Update_board_by_one_step");
        if ((number_of_gems_to_move > 0) || (number_of_new_gems_to_create > 0) || (number_of_elements_to_damage + number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage_with_bonus > 0))
        {
            if (number_of_elements_to_damage > 0)
            {
                Cancell_hint();
                Calculate_score();
            }

            for (int y = _Y_tiles - 1; y >= 0; y--)//from bottom to top
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    //Debug.Log(x + "," + y + " = " + board_array_master[x,y,11]);

                    board_array_master[x, y, 13] = 0;//no checked

                    if (board_array_master[x, y, 11] == 1)//destroy
                    {
                        script_tiles_array[x, y].Explosion();
                    }
                    else if (board_array_master[x, y, 11] == 2)//creation
                    {
                        //Debug.Log(x + "," + y);
                        script_tiles_array[x, y].CreationStart();
                    }
                    else if (board_array_master[x, y, 11] >= 3)//falling
                    {
                        //Debug.Log(x + "," + y + " = " + "falling gem");
                        script_tiles_array[x, y].Fall_by_one_step(board_array_master[x, y, 11] - 3);
                    }
                }
            }

        }
        else
        {
            if (!gem_falling_ongoing)
                Check_secondary_explosions();
        }
    }

    void Start_gem_fall()//call from "board_C.turnMovement.Start_update_board()"
    {
        Debug.Log("Start_gem_fall " + number_of_gems_to_move + " " + number_of_new_gems_to_create);
        if ((number_of_gems_to_move > 0) || (number_of_new_gems_to_create > 0))
        {
            gem_falling_ongoing = true;

            for (int y = _Y_tiles - 1; y >= 0; y--)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 0] > -1)//if there is a tile
                    {
                        //search free tiles
                        if ((board_array_master[x, y, 1] == -99)  //if this tile is empty
                             && (board_array_master[x, y, 13] <= 0)) //and not yet checked
                        {
                            script_tiles_array[x,y].Make_fall_all_free_gems_over_this_empty_tile();
                        }
                    }
                }
            }

        }
        else
        {
            if (!gem_falling_ongoing)
            {
                Check_ALL_possible_moves();
            }
        }
    }

}
