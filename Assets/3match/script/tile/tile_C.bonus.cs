using UnityEngine;
using System.Collections;

public partial class tile_C {



    public void Try_to_use_bonus_on_this_tile()
    {
        //Debug.Log("***___Try to use " + board.bonus_select.ToString()  + _x + "," + _y );
        if (board.board_array_master[_x, _y, 1] >= 0) //click on a tile that have something in itself
        {
            switch (board.bonus_select)
            {
                case Bonus.DestroyOne:
                    Destroy_one(true);
                    break;

                case Bonus.SwitchGemTeleport:
                    Gems_teleport();
                    break;

                case Bonus.Destroy3x3:
                    Destroy_3x3(true);
                    break;

                case Bonus.DestroyHorizontal:
                    Destroy_horizontal(true);
                    break;

                case Bonus.DestroyVertical:
                    Destroy_vertical(true);
                    break;

                case Bonus.DestroyHorizontalAndVertical:
                    Destroy_horizontal_and_vertical(true);
                    break;

                case Bonus.DestroyAllGemsWithThisColor:
                    Destroy_all_gems_with_this_color();
                    break;
            }
            /*
			if (board.bonus_select != Board_C.bonus.switch_gem_teleport)
				{
				if (board.player_turn)
					board.gui_bonus_ico_array[board.slot_bonus_ico_selected].GetComponent<bonus_button>().Reset_fill();
				else
					board.gui_enemy_bonus_ico_array[board.enemy_chosen_bonus_slot].GetComponent<bonus_button>().Reset_fill();
				}*/
        }
    }

    public void Enemy_click_on_this_bonus()
    {
        Trigger_bonus(true);
    }

    public void Trigger_bonus(bool start_explosion)
    {
        if (board.board_array_master[_x, _y, 11] == 111)
            return;

        //Debug.Log(_x + "," + _y + " Trigger_bonus: " + start_explosion + " = " + board.board_array_master[_x, _y, 4]);
        if (board.board_array_master[_x, _y, 4] == 1) //active hammer (need a click on a target gem to be activated)
        {
        }
        else if (board.board_array_master[_x, _y, 4] == 2) //switch_gem_teleport
        {
        }
        else if (board.board_array_master[_x, _y, 4] == 3) //explode bomb
        {
            Destroy_3x3(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 4) //destroy_horizontal
        {
            Destroy_horizontal(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 5) //destroy_vertical
        {
            Destroy_vertical(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 6) //destroy_horizontal_and_vertical
        {
            Destroy_horizontal_and_vertical(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 7) //destroy_all_gem_with_this_color (need a click on a target gem to be activated)
        {
        }
        else if (board.board_array_master[_x, _y, 4] == 8) //give_more_time
        {
            Give_more_time(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 9) //give_more_moves
        {
            Give_more_moves(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 10) //heal
        {
            Heal_me(start_explosion);
        }
        else if (board.board_array_master[_x, _y, 4] == 11) //damage
        {
            Damage_opponent(start_explosion);
        }
    }



    void Gems_teleport()
    {
        if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] < 9)) //is a gem
        {
            if (board.board_array_master[_x, _y, 3] == 0) //no padlock
            {
                if (board.main_gem_selected_x == -10) //select first gem
                {
                    board.main_gem_selected_x = _x;
                    board.main_gem_selected_y = _y;
                    board.main_gem_color = board.board_array_master[_x, _y, 1];
                    //Debug.Log("teleport select first gem: " + _x + "," + _y);

                    //If Enemy turn, set the second click:
                    if (!board.player_turn)
                    {
                        //Debug.Log("secondary teleport gem: " + board.enemy_bonus_click_2_x + "," + board.enemy_bonus_click_2_y);
                        board.script_tiles_array[board.enemy_bonus_click_2_x, board.enemy_bonus_click_2_y].Invoke("Try_to_use_bonus_on_this_tile", board.globalRules.enemy_move_delay);
                    }
                }
                else //select second gem
                {
                    if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y == _y))
                    {
                        //you have click on the same gem, so deselect it
                        board.main_gem_selected_x = -10;
                        board.main_gem_selected_y = -10;
                        board.main_gem_color = -10;
                    }
                    else
                    {
                        //board.minor_gem_destination_to_x = _x;
                        //board.minor_gem_destination_to_y = _y;
                        board.minor_gem_color = board.board_array_master[_x, _y, 1];
                        Debug.Log("teleport select second gem: " + _x + "," + _y);

                        //activate teleport
                        board.number_of_gems_to_mix = 2;
                        board.player_can_move = false;

                        //change gems
                        board.board_array_master[_x, _y, 1] = board.main_gem_color;
                        board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 1] = board.minor_gem_color;

                        board.audioManager.Play_bonus_sfx(2);

                        //update gem
                        StartCoroutine(Shuffle_update());

                        board.script_tiles_array[board.main_gem_selected_x, board.main_gem_selected_y].StartCoroutine(board.script_tiles_array[board.main_gem_selected_x, board.main_gem_selected_y].Shuffle_update());


                    }
                }
            }
        }

    }

    void Heal_me(bool start_explosion)
    {
        board.player_can_move = false;
        board.board_array_master[_x, _y, 4] = 0;

        board.Heal_me(board.activeCharacter.myCharacter.heal_me_hp_bonus);

        if (start_explosion)
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

        if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
            board.Update_on_board_bonus_count();

        if (start_explosion)
        {
            board.audioManager.Play_bonus_sfx(10);
            board.Order_to_gems_to_explode();
        }

    }

    void Damage_opponent(bool start_explosion)
    {
        board.player_can_move = false;
        board.board_array_master[_x, _y, 4] = 0;

        board.Damage_opponent(board.activeCharacter.myCharacter.damage_opponent_bonus);

        if (start_explosion)
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

        if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
            board.Update_on_board_bonus_count();

        if (start_explosion)
        {
            board.audioManager.Play_bonus_sfx(11);
            board.Order_to_gems_to_explode();
        }

    }

    void Give_more_moves(bool start_explosion)
    {
        board.player_can_move = false;
        board.board_array_master[_x, _y, 4] = 0;
        board.Gain_turns(board.globalRules.add_moves_bonus);

        if (start_explosion)
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

        if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
            board.Update_on_board_bonus_count();

        if (start_explosion)
        {
            board.audioManager.Play_bonus_sfx(9);
            board.Order_to_gems_to_explode();
        }

    }

    void Give_more_time(bool start_explosion)
    {
        print("Give_more_time: " + start_explosion);
        board.player_can_move = false;
        board.board_array_master[_x, _y, 4] = 0;
        board.Add_time_bonus(board.globalRules.add_time_bonus);

        if (start_explosion)
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

        if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
            board.Update_on_board_bonus_count();

        if (start_explosion)
        {
            print("Give_more_time > explosion");
            board.audioManager.Play_bonus_sfx(8);
            board.Order_to_gems_to_explode();
        }

        board.uIManager.Reset_charge_fill();
        board.uIManager.Update_inventory_bonus(8, -1);

    }



    void Destroy_one(bool start_explosion)
    {

        if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) && (board.board_array_master[_x, _y, 4] >= -100) //it is a gem or junk
            && (board.board_array_master[_x, _y, 4] <= 0))//but not a bonus
        {

            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            board.uIManager.Reset_charge_fill();
            board.uIManager.Update_inventory_bonus(1, -1);

            board.garbageManager.RecycleBonusDestroyOneFX(_x, _y);

            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

            if (start_explosion)
            {
                board.audioManager.Play_bonus_sfx(1);
                board.Order_to_gems_to_explode();
            }

        }
        else if ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 70)) //it is a block
        {
            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            board.uIManager.Reset_charge_fill();
            board.uIManager.Update_inventory_bonus(1, -1);

            board.garbageManager.RecycleBonusDestroyOneFX(_x, _y);

            board.number_of_elements_to_damage++;
            Damage_block();
        }

        board.Update_on_board_bonus_count();
    }

    void Destroy_3x3(bool start_explosion)
    {
        if (((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) && (board.board_array_master[_x, _y, 4] >= -100))  //it is a gem or junk
            || ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 70)))//it is a block
        {
            if ((board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory) && (board.board_array_master[_x, _y, 4] > 0))
                return;

            board.board_array_master[_x, _y, 4] = 0;
            if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
                board.Update_on_board_bonus_count();

            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            for (int y = (_y - 1); y < ((_y - 1) + 3); y++)
            {
                if ((y >= 0) && (y < board._Y_tiles))
                {
                    for (int x = (_x - 1); x < ((_x - 1) + 3); x++)
                    {
                        if ((x >= 0) && (x < board._X_tiles))
                        {
                            if (board.board_array_master[x, y, 1] >= 0) //if this tile have something
                            {
                                if (((board.board_array_master[x, y, 1] >= 0) && (board.board_array_master[x, y, 1] <= 9) && (board.board_array_master[x, y, 4] >= -100))  //it is a gem or junk 
                                    || ((board.board_array_master[x, y, 1] >= 41) && (board.board_array_master[x, y, 1] < 70))) //it is a block
                                {
                                    if (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory)
                                        board.Annotate_explosions(x, y, Board_C.ExplosionCause.bonus);
                                    else
                                    {
                                        if (board.board_array_master[x, y, 4] <= 0)
                                            board.Annotate_explosions(x, y, Board_C.ExplosionCause.bonus);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            board.uIManager.Reset_charge_fill();
            board.uIManager.Update_inventory_bonus(3, -1);

            if ((board.number_of_elements_to_damage_with_bonus > 0) && (start_explosion))//if there is something to explode
            {
                board.garbageManager.RecycleBonusDestroy3x3FX(_x, _y);

                board.audioManager.Play_bonus_sfx(3);
                board.Order_to_gems_to_explode();
            }

        }
    }

    void Destroy_horizontal(bool start_explosion)
    {
        if (((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) && (board.board_array_master[_x, _y, 4] >= -100))  //is a gem or junk
            || ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 70)))//it is a block
        {
            if ((board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory) && (board.board_array_master[_x, _y, 4] > 0))
                return;

            board.board_array_master[_x, _y, 4] = 0;
            if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
                board.Update_on_board_bonus_count();

            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

            Destroy_on_the_right_side();
            Destroy_on_the_left_side();

            board.uIManager.Reset_charge_fill();
            board.uIManager.Update_inventory_bonus(4, -1);

            if ((board.number_of_elements_to_damage_with_bonus > 0) && (start_explosion))//if there is something to explode
            {
                board.garbageManager.RecycleBonusDestroyHorizontalFX(_x, _y, fx_end_r, fx_end_l);


                board.audioManager.Play_bonus_sfx(4);
                board.Order_to_gems_to_explode();
            }



        }
    }

    void Destroy_on_the_right_side()
    {
        fx_end_r = 0;

        //decide if stop when it an empty tile or not
        int tile_target = 0;
        if (!board.globalRules.linear_explosion_stop_against_empty_space)
            tile_target = -1;

        for (int x = (_x + 1); x < board._X_tiles; x++)
        {
            if (board.board_array_master[x, _y, 0] >= tile_target)
            {
                if (board.board_array_master[x, _y, 1] > -99) //if there is something in this tile
                {
                    if ((board.board_array_master[x, _y, 1] >= 0) && (board.board_array_master[x, _y, 1] <= 9))//if gem or special gem
                    {
                        if (board.board_array_master[x, _y, 4] == 0) //it is a normal gem
                        {
                            board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                            fx_end_r++;
                        }
                        else if (board.board_array_master[x, _y, 4] > 0) //it is a bonus
                        {
                            if (board.globalRules.linear_explosion_stop_against_bonus)
                                break;
                            else
                            {
                                if (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory)
                                {
                                    board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                                    fx_end_r++;
                                }
                                else
                                {
                                    fx_end_r++;
                                }
                            }
                        }
                        else //is a special gem
                        {
                            if (board.board_array_master[x, _y, 4] == -200) //token
                            {
                                if (board.globalRules.linear_explosion_stop_against_token)
                                    break;
                                else
                                    fx_end_r++;
                            }
                            else if (board.board_array_master[x, _y, 4] == -100) //junk
                            {
                                board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                                fx_end_r++;
                            }
                        }
                    }
                    else if ((board.board_array_master[x, _y, 1] > 40) && (board.board_array_master[x, _y, 1] < 70)) //it is a block
                    {
                        if (board.globalRules.linear_explosion_stop_against_block)
                            break;
                        else
                        {
                            board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                            fx_end_r++;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("hit mysterious stuff at " + x + "," + _y + " = " + board.board_array_master[x, _y, 1]);
                    }
                }
                else //this tile is empty
                {
                    if (board.board_array_master[x, _y, 0] == -1) //no tile here
                    {
                        if (board.globalRules.linear_explosion_stop_against_empty_space)
                            break;
                        else
                            fx_end_r++;
                    }
                    else
                        fx_end_r++;
                }
            }
            else
                break;

        }
    }

    void Destroy_on_the_left_side()
    {

        fx_end_l = 0;

        //decide if stop when it an empty tile or not
        int tile_target = 0;
        if (!board.globalRules.linear_explosion_stop_against_empty_space)
            tile_target = -1;

        for (int x = (_x - 1); x >= 0; x--)
        {
            if (board.board_array_master[x, _y, 0] >= tile_target)
            {
                if (board.board_array_master[x, _y, 1] > -99) //if there is something in this tile
                {
                    if ((board.board_array_master[x, _y, 1] >= 0) && (board.board_array_master[x, _y, 1] <= 9))//if gem or special gem
                    {
                        if (board.board_array_master[x, _y, 4] == 0) //it is a normal gem
                        {
                            board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                            fx_end_l++;
                        }
                        else if (board.board_array_master[x, _y, 4] > 0) //it is a bonus
                        {
                            if (board.globalRules.linear_explosion_stop_against_bonus)
                                break;
                            else
                            {
                                if (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory)
                                {
                                    board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                                    fx_end_l++;
                                }
                                else
                                {
                                    fx_end_l++;
                                }
                            }
                        }
                        else //is a special gem
                        {
                            if (board.board_array_master[x, _y, 4] == -200) //token
                            {
                                if (board.globalRules.linear_explosion_stop_against_token)
                                    break;
                                else
                                    fx_end_l++;
                            }
                            else if (board.board_array_master[x, _y, 4] == -100) //junk
                            {
                                board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                                fx_end_l++;
                            }
                        }
                    }
                    else if ((board.board_array_master[x, _y, 1] > 40) && (board.board_array_master[x, _y, 1] < 70)) //it is a block
                    {
                        if (board.globalRules.linear_explosion_stop_against_block)
                            break;
                        else
                        {
                            board.Annotate_explosions(x, _y, Board_C.ExplosionCause.bonus);
                            fx_end_l++;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("hit mysterious stuff at " + x + "," + _y + " = " + board.board_array_master[x, _y, 1]);
                    }
                }
                else //this tile is empty
                {
                    if (board.board_array_master[x, _y, 0] == -1) //no tile here
                    {
                        if (board.globalRules.linear_explosion_stop_against_empty_space)
                            break;
                        else
                            fx_end_l++;
                    }
                    else
                        fx_end_l++;
                }
            }
            else
                break;

        }

    }

    void Destroy_above()
    {

        fx_end_up = 0;

        //decide if stop when it an empty tile or not
        int tile_target = 0;
        if (!board.globalRules.linear_explosion_stop_against_empty_space)
            tile_target = -1;

        for (int y = (_y - 1); y >= 0; y--)
        {
            if (board.board_array_master[_x, y, 0] >= tile_target)
            {
                if (board.board_array_master[_x, y, 1] > -99) //if there is something in this tile
                {
                    if ((board.board_array_master[_x, y, 1] >= 0) && (board.board_array_master[_x, y, 1] <= 9))//if gem or special gem
                    {
                        if (board.board_array_master[_x, y, 4] == 0) //it is a normal gem
                        {
                            board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                            fx_end_up++;
                        }
                        else if (board.board_array_master[_x, y, 4] > 0) //it is a bonus
                        {
                            if (board.globalRules.linear_explosion_stop_against_bonus)
                                break;
                            else
                            {
                                if (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory)
                                {
                                    board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                                    fx_end_up++;
                                }
                                else
                                {
                                    fx_end_up++;
                                }
                            }
                        }
                        else //is a special gem
                        {
                            if (board.board_array_master[_x, y, 4] == -200) //token
                            {
                                if (board.globalRules.linear_explosion_stop_against_token)
                                    break;
                                else
                                    fx_end_up++;
                            }
                            else if (board.board_array_master[_x, y, 4] == -100) //junk
                            {
                                board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                                fx_end_up++;
                            }
                        }
                    }
                    else if ((board.board_array_master[_x, y, 1] > 40) && (board.board_array_master[_x, y, 1] < 70)) //it is a block
                    {
                        if (board.globalRules.linear_explosion_stop_against_block)
                            break;
                        else
                        {
                            board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                            fx_end_up++;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("hit mysterious stuff at " + _x + "," + y + " = " + board.board_array_master[_x, y, 1]);
                    }
                }
                else //this tile is empty
                {
                    if (board.board_array_master[_x, y, 0] == -1) //no tile here
                    {
                        if (board.globalRules.linear_explosion_stop_against_empty_space)
                            break;
                        else
                            fx_end_up++;
                    }
                    else
                        fx_end_up++;
                }
            }
            else
                break;

        }

    }

    void Destroy_beneath()
    {

        fx_end_down = 0;

        //decide if stop when it an empty tile or not
        int tile_target = 0;
        if (!board.globalRules.linear_explosion_stop_against_empty_space)
            tile_target = -1;

        for (int y = (_y + 1); y < board._Y_tiles; y++)
        {
            if (board.board_array_master[_x, y, 0] >= tile_target)
            {
                if (board.board_array_master[_x, y, 1] > -99) //if there is something in this tile
                {
                    if ((board.board_array_master[_x, y, 1] >= 0) && (board.board_array_master[_x, y, 1] <= 9))//if gem or special gem
                    {
                        if (board.board_array_master[_x, y, 4] == 0) //it is a normal gem
                        {
                            board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                            fx_end_down++;
                        }
                        else if (board.board_array_master[_x, y, 4] > 0) //it is a bonus
                        {
                            if (board.globalRules.linear_explosion_stop_against_bonus)
                                break;
                            else
                            {
                                if (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory)
                                {
                                    board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                                    fx_end_down++;
                                }
                                else
                                {
                                    fx_end_down++;
                                }
                            }
                        }
                        else //is a special gem
                        {
                            if (board.board_array_master[_x, y, 4] == -200) //token
                            {
                                if (board.globalRules.linear_explosion_stop_against_token)
                                    break;
                                else
                                    fx_end_down++;
                            }
                            else if (board.board_array_master[_x, y, 4] == -100) //junk
                            {
                                board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                                fx_end_down++;
                            }
                        }
                    }
                    else if ((board.board_array_master[_x, y, 1] > 40) && (board.board_array_master[_x, y, 1] < 70)) //it is a block
                    {
                        if (board.globalRules.linear_explosion_stop_against_block)
                            break;
                        else
                        {
                            board.Annotate_explosions(_x, y, Board_C.ExplosionCause.bonus);
                            fx_end_down++;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("hit mysterious stuff at " + _x + "," + y + " = " + board.board_array_master[_x, y, 1]);
                    }
                }
                else //this tile is empty
                {
                    if (board.board_array_master[_x, y, 0] == -1) //no tile here
                    {
                        if (board.globalRules.linear_explosion_stop_against_empty_space)
                            break;
                        else
                            fx_end_down++;
                    }
                    else
                        fx_end_down++;
                }
            }
            else
                break;

        }

    }

    void Destroy_vertical(bool start_explosion)
    {
        if (((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) && (board.board_array_master[_x, _y, 4] >= -100))  //it is a gem or junk
            || ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 70)))//it is a block
        {
            if ((board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory) && (board.board_array_master[_x, _y, 4] > 0))
                return;

            board.board_array_master[_x, _y, 4] = 0;
            if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
                board.Update_on_board_bonus_count();

            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

            Destroy_beneath();
            Destroy_above();

            board.uIManager.Reset_charge_fill();
            board.uIManager.Update_inventory_bonus(5, -1);

            if ((board.number_of_elements_to_damage_with_bonus > 0) && (start_explosion))//if there is something to explode
            {
                board.garbageManager.RecycleBonusDestroyVerticalFX(_x, _y, fx_end_up, fx_end_down);


                board.audioManager.Play_bonus_sfx(5);
                board.Order_to_gems_to_explode();

            }

        }

    }

    void Destroy_horizontal_and_vertical(bool start_explosion)
    {
        //Debug.Log("Destroy_horizontal_and_vertical");
        if (((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) && (board.board_array_master[_x, _y, 4] >= -100))  //is a gem or junk
            || ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 70)))//it is a block
        {
            if ((board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory) && (board.board_array_master[_x, _y, 4] > 0))
                return;

            board.board_array_master[_x, _y, 4] = 0;
            if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.after_charge) && (board.myRuleset.trigger_by_select != Ruleset.trigger_by.inventory))
                board.Update_on_board_bonus_count();

            board.player_can_move = false;
            board.cursor.gameObject.SetActive(false);

            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.bonus);

            Destroy_on_the_right_side();
            Destroy_on_the_left_side();

            Destroy_beneath();
            Destroy_above();

            board.uIManager.Reset_charge_fill();
            board.uIManager.Update_inventory_bonus(6, -1);

            //Debug.Log("board.number_of_elements_to_damage: " + board.number_of_elements_to_damage + " start_explosion: " + start_explosion);
            if ((board.number_of_elements_to_damage_with_bonus > 0) && (start_explosion))//if there is something to explode
            {
                //Debug.Log("if there is something to explode");
                board.garbageManager.RecycleBonusDestroyHorizontalAndVerticalFX(_x, _y, fx_end_up, fx_end_down, fx_end_r, fx_end_l);

                board.audioManager.Play_bonus_sfx(6);
                board.Order_to_gems_to_explode();
                //board.Update_on_board_bonus_count();
            }
        }

    }

    void Destroy_all_gems_with_this_color()
    {
        if (board.board_array_master[_x, _y, 4] != 0)
            return;

        //Debug.Log("Destroy_all_gems_with_this_color (tile version)");
        board.player_can_move = false;
        board.cursor.gameObject.SetActive(false);


        //board.elements_to_damage_array = new GameObject[board._X_tiles * board._Y_tiles];

        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                //Debug.Log("check " + x +","+y);
                if (board.board_array_master[x, y, 1] == board.board_array_master[_x, _y, 1]) //if this gem have my same color
                {
                    board.Annotate_explosions(x, y, Board_C.ExplosionCause.bonus);
                }
            }
        }

        board.uIManager.Reset_charge_fill();
        board.uIManager.Update_inventory_bonus(7, -1);

        board.audioManager.Play_bonus_sfx(7);
        board.Order_to_gems_to_explode();
        board.Update_on_board_bonus_count();
    }


    public void Change_gem_in_bonus()
    {
        //Debug.Log("Change_gem_in_bonus " + _x + "," + _y + " my color: " + board.board_array_master[_x,_y,1] + "bonus: " + after_explosion_I_will_become_this_bonus);
        if (board.board_array_master[_x, _y, 1] <= 4)//if this is a normal gem
            Update_gems_score();

        //create bonus
        Create_special_element(after_explosion_I_will_become_this_bonus);

        StartCoroutine("Return_to_normal_size");


    }

    void Create_special_element(int element_id)
    {
        board.board_array_master[_x, _y, 4] = element_id;
        after_explosion_I_will_become_this_bonus = 0;
        board.board_array_master[_x, _y, 1] = 9;//neutre = don't explode if 3 in a row
        board.board_array_master[_x, _y, 10] = 1;//this thing can fall

        //SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
        if (element_id > 0) //bonus
            {
            board.number_of_bonus_on_board++;
            myContent.mySpriteRenderer.sprite = board.myTheme.on_board_bonus_sprites[element_id];
            }
        else if (element_id == -100)//junk
            {
            board.number_of_junk_on_board++;
            myContent.mySpriteRenderer.sprite = board.myTheme.junk;
            }
        else if (element_id == -200)//token
        {
            myContent.mySpriteRenderer.sprite = board.myTheme.token;
            board.number_of_token_emitted++;
            board.number_of_token_on_board++;
        }
    }
}
