using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Board_C : MonoBehaviour {



    public void Gain_turns(int quantity)
    {
        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves || myRuleset.versus)
        {
            activeCharacter.myCharacter.currentMovesLeft += quantity;
            if (globalRules.praise_the_player && globalRules.for_gain_a_turn)
                globalRules.praise_script.Gain_a_turn(activeCharacter.myUI.gui_name_text.text, quantity);

            turn_gained = true;
            uIManager.Update_left_moves_text();
        }
    }



    void Check_new_gem_to_generate() //call from: Board_C.freeMovement.BoardUpdate(), Board_C.turnMovement.Start_update_board()
    {
        if (myRuleset.gem_emitter_rule == Ruleset.gem_emitter.off)
        {
            number_of_new_gems_to_create = 0;
            return;
        }

        for (int i = 0; i < number_of_tiles_leader; i++)
        {
            GenerateGemInThisTile(tiles_leader_array[i]._x, tiles_leader_array[i]._y);

            /*
            if (board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 1] == -99)//empty 
            {
                if (board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 6 || board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 111 || board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 222 || board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 333)//explosion or creation falling animation ongoing
                    continue;

                board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] = 2;//gem creation
                board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 13] = 1;
                number_of_new_gems_to_create++;


                //empty tile are reserved to this generated new gem (this is needed only when the explosion is a column on the top of the board, to prevet diagona falling from adiacent columns)
                for (int yy = tiles_leader_array[i]._y + 1; yy < _Y_tiles; yy++)
                {
                    if ((board_array_master[tiles_leader_array[i]._x, yy, 0] > -1) && (board_array_master[tiles_leader_array[i]._x, yy, 1] == -99))
                    {
                        board_array_master[tiles_leader_array[i]._x, yy, 13] = 1;
                        //tiles_array[tiles_leader_array[i]._x, yy].GetComponent<Renderer>().material.color = Color.yellow;
                    }
                    else
                        break;
                }
            }
            */
        }

        //do the same under the generative blocks
        for (int i = 0; i < generativeBlockInfos.Count; i++)
        {
            if (!generativeBlockInfos[i].generatorIsOn)
                continue;

            if (generativeBlockInfos[i].myGenerativeTargertPosition.y < _Y_tiles)
                GenerateGemInThisTile(generativeBlockInfos[i].myGenerativeTargertPosition.x, generativeBlockInfos[i].myGenerativeTargertPosition.y);
        }

    }

    void GenerateGemInThisTile(int x, int y)
    {
        if (board_array_master[x, y, 1] == -99)//empty 
        {
            if (board_array_master[x, y, 11] == 6 || board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                return;

            board_array_master[x, y, 11] = 2;//gem creation
            board_array_master[x, y, 13] = 1;
            number_of_new_gems_to_create++;


            //empty tile are reserved to this generated new gem (this is needed only when the explosion is a column on the top of the board, to prevet diagona falling from adiacent columns)
            for (int yy = y + 1; yy < _Y_tiles; yy++)
            {
                if ((board_array_master[x, yy, 0] > -1) && (board_array_master[x, yy, 1] == -99))
                {
                    board_array_master[x, yy, 13] = 1;
                }
                else
                    break;
            }
        }
    }


    #region falling
    public void Check_vertical_falling()
    {

        for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                
                //if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                if (board_array_master[x, y, 11] != 0 || isSwitching[x, y])
                    continue;

                //this (board_array_master[x, y, 13] == 0) cause bug because can have board_array_master[x, y, 13] == 1 and the tile under empty
                if (/*(board_array_master[x, y, 13] == 0) &&*/ (board_array_master[x, y, 10] == 1))//this could be fall
                {
                    //check if have an empty tile under
                    if ((board_array_master[x, y + 1, 0] > -1) && (board_array_master[x, y + 1, 1] == -99)
                       || board_array_master[x, y + 1, 11] == 3) //or something falling

                    {
                        //vertical fall
                        //tiles_array[x,y].GetComponent<Renderer>().material.color = Color.gray;
                        board_array_master[x, y, 11] = 3;//vertical fall
                        board_array_master[x, y, 13] = 1;//tile checked
                        number_of_gems_to_move++;

                        //all empty tiles under this gem are reserved to it
                        for (int yy = y + 1; yy < _Y_tiles; yy++)
                        {
                            if ((board_array_master[x, yy, 0] > -1) && (board_array_master[x, yy, 1] == -99))
                            {
                                //Debug.Log(x + "," + yy + "   There is something to move");
                                board_array_master[x, yy, 13] = 1;
                                //script_tiles_array[x,yy].GetComponent<Renderer>().material.color = Color.cyan;
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }
    }

    void Check_diagonal_falling()
    {
        if (!myRuleset.diagonal_falling)
            return;

        //print("Check_diagonal_falling() - " + calledFrom);
        diagonal_falling_preference_direction_R = !diagonal_falling_preference_direction_R;
        for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
        {
            for (int x = 0; x < _X_tiles; x++)
            {

                //if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                if (board_array_master[x, y, 11] != 0)
                    continue;

                if ((board_array_master[x, y, 13] == 0) && (board_array_master[x, y, 10] == 1))//this could be fall
                {

                    if (diagonal_falling_preference_direction_R)
                    {
                        Diagonal_fall_R(x, y);
                        Diagonal_fall_L(x, y);
                    }
                    else
                    {
                        Diagonal_fall_L(x, y);
                        Diagonal_fall_R(x, y);
                    }
                }
            }
        }
    }



    void Diagonal_fall_R(int x, int y)
    {
        if (x + 1 < _X_tiles)
        {
            if (script_tiles_array[x + 1, y + 1] == null)
                return;

            if (script_tiles_array[x + 1, y + 1].myContent != null)
                return;

            if (board_array_master[x + 1, y, 12] > 0 || board_array_master[x + 1, y + 1, 12] > 0)//don't wall were a new gem can be created
                return;

            /*
            //if my right tile have someting that prevent the vertical falling, you can use diagonal fall
            if (board_array_master[x + 1, y, 3] == 0) //is not reastrained
                return;
            */

            if (board_array_master[x + 1, y, 10] > 0)//can fall
                return;

            //Debug.Log("Diagonal_fall_R " + x + "," + y);

            if ((board_array_master[x + 1, y + 1, 0] > -1) 
                &&(board_array_master[x + 1, y + 1, 13] == 0 
                && board_array_master[x + 1, y + 1, 1] == -99) //empty
                 && board_array_master[x + 1, y + 1, 11] == 0)// || (board_array_master[x + 1, y + 1, 13] == 1 && board_array_master[x+1, y+1, 11] == 4)) //or something falling R
                    {
                    //tiles_array[x,y].GetComponent<Renderer>().material.color = Color.red;
                    board_array_master[x, y, 11] = 4;//R falling
                    board_array_master[x, y, 13] = 1;
                    number_of_gems_to_move++;

                    board_array_master[x + 1, y + 1, 13] = 1;//reserved target position
                    }
            
        }
    }

    public void Diagonal_fall_L(int x, int y)
    {

        if (x - 1 >= 0)
        {

            if (script_tiles_array[x - 1, y + 1] == null)
                return;


            if (script_tiles_array[x - 1, y + 1].myContent != null)
                return;



            if (board_array_master[x - 1, y, 12] > 0|| board_array_master[x - 1, y + 1, 12] > 0)//don't wall were a new gem can be created
                return;

            //if my right tile have someting that prevent the vertical falling, you can use diagonal fall
            /*
            if (board_array_master[x - 1, y, 3] == 0) //is not reastrained
                return;
            */


            if (board_array_master[x - 1, y, 10] > 0)//can fall
                return;



            //Debug.Log("Diagonal_fall_L " + x + "," + y);


            if ((board_array_master[x - 1, y + 1, 0] > -1)
                    && (board_array_master[x - 1, y + 1, 13] == 0)
                    && (board_array_master[x - 1, y + 1, 1] == -99)
                    && (board_array_master[x - 1, y + 1, 11] == 0)
                    )
                {
                    //if (tiles_array[x,y].GetComponent<Renderer>().material.color != Color.red)
                    //	tiles_array[x,y].GetComponent<Renderer>().material.color = Color.blue;
                    //else
                    //	tiles_array[x,y].GetComponent<Renderer>().material.color = Color.magenta;
                    board_array_master[x, y, 11] = 5;//L falling
                    board_array_master[x, y, 13] = 1;
                    number_of_gems_to_move++;

                    board_array_master[x - 1, y + 1, 13] = 1;//reserved target position
                }
            }
        
    }
#endregion


    #region explosion
    public void Annotate_explosions(int xx, int yy, ExplosionCause explosion_caused_by)// = ExplosionCause.secondayExplosion)
    {
        //print("Annotate_explosions " + xx + "," + yy + " ... " + board_array_master[xx, yy, 11] + " cause: " + explosion_caused_by);

        if (board_array_master[xx, yy, 11] == 111 || board_array_master[xx, yy, 11] == 222 || board_array_master[xx, yy, 11] == 333)//explosion or creation falling animation ongoing
            return;

        if (board_array_master[xx, yy, 11] == 0 || (board_array_master[xx, yy, 11] == 6 && (explosion_caused_by == ExplosionCause.switchingGems || explosion_caused_by == ExplosionCause.bonus))) // if this explosion not is already marked
        {
            //Debug.Log("Annotate_explosions " + xx +"," + yy + " *** number_of_elements_to_damage: " + number_of_elements_to_damage + " *** elements_to_damage_array: " + elements_to_damage_array.Length);
            if (explosion_caused_by == ExplosionCause.switchingGems)
            {
                board_array_master[xx, yy, 11] = 66;
                forceExecuteChangesUntilExplosionIsSee = true;

                reserved_for_primary_explosion.Add(script_tiles_array[xx, yy]);
                number_of_elements_to_damage_with_SwitchingGems++;
            }
            else
            {
                board_array_master[xx, yy, 11] = 1;// this gem explode
                forceExecuteChangesUntilExplosionIsSee = true;

                if (explosion_caused_by == ExplosionCause.secondayExplosion)
                    number_of_elements_to_damage++;
                else if (explosion_caused_by == ExplosionCause.bonus)
                    number_of_elements_to_damage_with_bonus++;
            }

            //elements_to_damage_array[number_of_elements_to_damage] = tiles_array[xx,yy];
            elements_to_damage_list.Add(script_tiles_array[xx, yy]);
            //Debug.Log("elements_to_damage_array["+number_of_elements_to_damage+"] = " + elements_to_damage_array[number_of_elements_to_damage]);


            //print("number_of_elements_to_damage: " + number_of_elements_to_damage);

            if (board_array_master[xx, yy, 3] > 0)
                number_of_padlocks_involved_in_explosion++;

            if (board_array_master[xx, yy, 4] > 0)//if this is a bonus, trigger it!
            {
                //Debug.Log("annotate explosion trigger bonus");
                script_tiles_array[xx, yy].Trigger_bonus(false);
            }
        }
    }


    void Search_secondary_exposions()
    {
        //Debug.Log("Search_secondary_exposions");
        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                    board_array_master[x, y, 13] = 0;//tile not checked

                if ((board_array_master[x, y, 11] != 0))
                    continue;

                if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9))//if there is a gem here
                {


                    if ((x + 1 < _X_tiles) && (x - 1 >= 0))//horizontal
                        if ((board_array_master[x, y, 1] == board_array_master[x + 1, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y, 1])) //if the adjacent tiles have gems with the same color
                        {
                            if (board_array_master[x + 1, y, 11] > 1 || board_array_master[x - 1, y, 11] > 1)//...but ignore if they are busy... > 1 instead of !=0 so takes in account the exploding gems
                                continue;

                            Annotate_explosions(x, y, ExplosionCause.secondayExplosion);


                            Annotate_explosions(x + 1, y, ExplosionCause.secondayExplosion);
                            if ((x + 2 < _X_tiles) && (board_array_master[x + 2, y, 1] == board_array_master[x, y, 1]))
                            {
                                Annotate_explosions(x + 2, y, ExplosionCause.secondayExplosion);
                            }

                            Annotate_explosions(x - 1, y, ExplosionCause.secondayExplosion);
                            if ((x - 2 >= 0) && (board_array_master[x - 2, y, 1] == board_array_master[x, y, 1]))
                            {
                                Annotate_explosions(x - 2, y, ExplosionCause.secondayExplosion);
                            }


                        }
                    if (((y + 1 < _Y_tiles) && (y - 1 >= 0)))//vertical
                        if ((board_array_master[x, y, 1] == board_array_master[x, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 1, 1]))//if the adjacent tiles have gems with the same color
                        {
                            
                            if (board_array_master[x, y + 1, 11] > 1 || board_array_master[x, y - 1, 11] > 1)//...but ignore if they are busy... > 1 instead of !=0 so takes in account the exploding gems
                                continue;

                            Annotate_explosions(x, y, ExplosionCause.secondayExplosion);

                            Annotate_explosions(x, y + 1, ExplosionCause.secondayExplosion);
                            if ((y + 2 < _Y_tiles) && (board_array_master[x, y + 2, 1] == board_array_master[x, y, 1]))
                                Annotate_explosions(x, y + 2, ExplosionCause.secondayExplosion);

                            Annotate_explosions(x, y - 1, ExplosionCause.secondayExplosion);
                            if ((y - 2 >= 0) && (board_array_master[x, y - 2, 1] == board_array_master[x, y, 1]))
                                Annotate_explosions(x, y - 2, ExplosionCause.secondayExplosion);
                        }
                }
            }
        }
    }





    void Assign_in_board_bonus(int xx, int yy, LatestSwitchInfo latestSwitchInfo)
    {
        Debug.Log("-- Assign_in_board_bonus --  " + latestSwitchInfo.is2x2Explosion + "    " + latestSwitchInfo.n_gems_exploded);
        if (myRuleset.choose_bonus_by_select == Ruleset.choose_bonus_by.explosion_magnitude)
        {
            if (latestSwitchInfo.is2x2Explosion)
            {
                //Debug.Log(xx+ ","+ yy +"   !!! SQUARE EXPLOSION !!! " + latestSwitchInfo.switchDiretion);

                if (latestSwitchInfo.switchDiretion == MyDirections.Up)
                    script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.Explosion2x2GivesThisBonus[0];
                else if (latestSwitchInfo.switchDiretion == MyDirections.Down)
                    script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.Explosion2x2GivesThisBonus[1];
                else if (latestSwitchInfo.switchDiretion == MyDirections.Left)
                    script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.Explosion2x2GivesThisBonus[2];
                else if (latestSwitchInfo.switchDiretion == MyDirections.Right)
                    script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.Explosion2x2GivesThisBonus[3];

            }
            else
            {
                //Debug.Log(xx + "," + yy + "   ___ LINEAR EXPLOSION ___ " + latestSwitchInfo.switchDiretion);

                if (latestSwitchInfo.switchDiretion ==  MyDirections.Down)
                    script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.big_explosion_down_give_bonus[latestSwitchInfo.n_gems_exploded - 4];
                else if (latestSwitchInfo.switchDiretion == MyDirections.Up)
                        script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.big_explosion_up_give_bonus[latestSwitchInfo.n_gems_exploded - 4];
                else if (latestSwitchInfo.switchDiretion == MyDirections.Left)
                    script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.big_explosion_left_give_bonus[latestSwitchInfo.n_gems_exploded - 4];
                else if (latestSwitchInfo.switchDiretion == MyDirections.Right)
                    script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.big_explosion_right_give_bonus[latestSwitchInfo.n_gems_exploded - 4];
            }

            //Debug.Log("bonus: " + script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus);
        }
        else if (myRuleset.choose_bonus_by_select == Ruleset.choose_bonus_by.gem_color)
        {

            if (latestSwitchInfo.is2x2Explosion)
                script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.ColorExplosion2x2GivesThisBonus[board_array_master[xx, yy, 1]];
            else
                script_tiles_array[xx, yy].after_explosion_I_will_become_this_bonus = (int)myRuleset.color_explosion_give_bonus[board_array_master[xx, yy, 1]];
        }
    }



    public void Order_to_gems_to_explode() //not use in step_by_step_update
    {
        //print("Order_to_gems_to_explode(): switch = " + number_of_elements_to_damage_with_SwitchingGems + " ... secondary = " + number_of_elements_to_damage + " ... bonus = " + number_of_elements_to_damage_with_bonus);
        if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
            return;


        Cancell_hint();
        Calculate_score();// (number_of_elements_to_damage_temp, "Order_to_gems_to_explode");

        for (int n = 0; n < elements_to_damage_list.Count; n++)
        {
            elements_to_damage_list[n].Explosion();
        }
        elements_to_damage_list.Clear();

    }



    void Check_bottom_tiles()
    {
        //Debug.Log("Check_bottom_tiles");
        for (int i = 0; i < number_of_bottom_tiles[current_board_orientation]; i++)
        {
            bottom_tiles_array[current_board_orientation, i].Check_the_content_of_this_tile();
        }
    }

    public void Check_secondary_explosions()//call from: Update_board_by_one_step(),  Board_C.freeMovement.BoardUpdate(), tile_C.shuffle.Check_if_shuffle_is_done(), tile_C.update.Check_if_gem_movements_are_all_done(), inventory_bonus_button.Activate(), 
    {
        //Debug.Log("Check_secondary_explosions");

        if (myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
        {
            all_explosions_are_completed = false;
            number_of_elements_to_damage = 0;
            number_of_elements_to_damage_with_SwitchingGems = 0;
            number_of_elements_to_damage_with_bonus = 0;
            number_of_padlocks_involved_in_explosion = 0;
            number_of_gems_to_move = 0;
            number_of_new_gems_to_create = 0;
            score_of_this_turn_move = 0;
            audioManager.play_this_bonus_sfx = -1;
        }

        //Debug.Log("number_of_bonus_on_board " + number_of_bonus_on_board);
        //Debug.Log("number_of_token_on_board " + number_of_token_on_board);
        //Debug.Log("number_of_junk_on_board " + number_of_junk_on_board);

        if (((myRuleset.trigger_by_select == Ruleset.trigger_by.inventory) && (number_of_bonus_on_board > 0))
            || (number_of_token_on_board > 0)
            || (number_of_junk_on_board > 0))
            Check_bottom_tiles();

        Search_secondary_exposions();



        if (number_of_elements_to_damage > 0)//if there is at least an explosion
        {
            n_combo++;
            Add_time_bonus(myRuleset.time_bonus_for_secondary_explosion);

            if (myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                Order_to_gems_to_explode();

            if (globalRules.praise_the_player && globalRules.for_secondary_explosions)
            {
                globalRules.praise_obj.SetActive(true);
                globalRules.praise_script.Combo_secondary_explosion(n_combo);
            }
        }
        else
        {
            if (myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                Check_ALL_possible_moves();
        }

    }

    #endregion
}
