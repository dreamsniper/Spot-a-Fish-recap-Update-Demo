using UnityEngine;
using System.Collections;
using System;


public partial class Board_C : MonoBehaviour
{

    int[] temp_enemy_gem_count;//this is use to evalutate bonus usefulness (collet gem AI)


    bool search_best_move;
    float big_move_value;

    float temp_bonus_value = 0;
    float best_bonus_value = 0;
    bool enemy_will_use_a_bonus;

    Vector2[] bonus_coordinate;

    [HideInInspector]public int enemy_chosen_bonus_slot;
    int enemy_bonus_click_1_x;
    int enemy_bonus_click_1_y;
    [HideInInspector] public int enemy_bonus_click_2_x;
    [HideInInspector] public int enemy_bonus_click_2_y;


    public enum enemy_AI
    {
        none = -1,
        random = 0,
        collect_gems_from_less_to_more = 1, //only to gem collect
        collect_gems_from_more_to_less = 2, //only to gem collect
        //dynamic_battle, //only to battle_mode
        advancedAI = 3,
        by_hand_setup = 4,
        just_deal_damage = 5
    }



    ArrayList[] gem_color;
    ArrayList available_directions;

    public enum enemy_AI_manual_setup
    {
        gem_0,
        gem_1,
        gem_2,
        gem_3,
        gem_4,
        gem_5,
        gem_6
    }
    enemy_AI_manual_setup[] enemy_AI_preference_order;
    int enemy_move_selected;    //main gem choice by the enemy
    int enemy_move_direction;//move direction of the main gem





    public void Enemy_play()//call from Update_turn_order_after_a_bad_move(), Board_C.tornMovement.UpdateTurn(), invenotry_bonus_button.Activate()
    {
        Debug.Log("Enemy_turn");
        /*
		if (myRuleset.lose_requirement_selected == lose_requirement.player_hp_is_zero)
			use_armor = true;
		else
			use_armor = false;*/

        enemy_move_selected = -1;
        enemy_will_use_a_bonus = false;

        if (enemy.myCharacter.chance_of_use_best_move == 0)
            search_best_move = false;
        else if (enemy.myCharacter.chance_of_use_best_move == 100)
            search_best_move = true;
        else
        {
            if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_best_move)
                search_best_move = true;
            else
                search_best_move = false;
        }

        switch (enemy.myCharacter.AI_selected)
        {
            case enemy_AI.random:
                if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_bonus)
                {
                    temp_enemy_gem_count = new int[myRuleset.gem_length];
                    Search_bonus(false);
                }
                if (enemy_will_use_a_bonus)
                {
                    Debug.Log("random AI - bonus found, so use bonus");
                    Enemy_use_bonus();
                }
                else
                {
                    Debug.Log("random AI - bonus not found");
                    //random choose among useful moves
                    enemy_move_selected = UnityEngine.Random.Range(0, number_of_gems_moveable - 1);

                    if (myRuleset.lose_requirement_selected != Ruleset.lose_requirement.enemy_collect_gems)
                    {
                        Enemy_select_main_gem(enemy_move_selected);
                    }
                    else
                    {
                        if (number_of_gems_moveable > 1)
                        {
                            //If I need the color choosed
                            if (enemy.myCharacter.numberOfGemsCollect[list_of_moves_possible[enemy_move_selected, 3]]
                                < myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[list_of_moves_possible[enemy_move_selected, 3]])
                            {
                                Enemy_select_main_gem(enemy_move_selected);
                            }
                            else//I try to find a better color
                            {
                                int temp_count = 0;
                                for (int i = 0; i < number_of_gems_moveable; i++)
                                {
                                    if ((enemy_move_selected + i) < number_of_gems_moveable)
                                    {
                                        temp_count++;
                                        if (enemy.myCharacter.numberOfGemsCollect[list_of_moves_possible[enemy_move_selected + i, 3]] < myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[list_of_moves_possible[enemy_move_selected + i, 3]])
                                        {
                                            enemy_move_selected = enemy_move_selected + i;
                                            Enemy_select_main_gem(enemy_move_selected);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (enemy.myCharacter.numberOfGemsCollect[list_of_moves_possible[i - temp_count, 3]] < myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[list_of_moves_possible[i - temp_count, 3]])
                                        {
                                            enemy_move_selected = i - temp_count;
                                            Enemy_select_main_gem(enemy_move_selected);
                                            break;
                                        }
                                    }
                                }
                                //If not exist a move with a color that I need, I make a random move
                                if (avatar_main_gem == null)
                                {
                                    Enemy_select_main_gem(enemy_move_selected);
                                }
                            }
                        }

                        else
                        {
                            Enemy_select_main_gem(enemy_move_selected);
                        }

                    }

                    Enemy_select_minor_gem();

                    Enemy_move();
                }



                break;


            case enemy_AI.collect_gems_from_less_to_more:

                Arrange_gems_by_necessity();
                Array.Reverse(enemy_AI_preference_order);
                Subdivide_moves_by_color();

                Enemy_decide_what_to_do();

                break;


            case enemy_AI.collect_gems_from_more_to_less:

                Arrange_gems_by_necessity();
                Subdivide_moves_by_color();

                Enemy_decide_what_to_do();

                break;

            /*
        case enemy_AI.dynamic_battle:

            if (use_armor)//search the gem with the color most effective againts the player armor
                Arrange_gems_by_effectiveness_against_player.armor();


            Subdivide_moves_by_color();

            Enemy_decide_what_to_do();

            break;
            */
            case enemy_AI.advancedAI:
                print("enemy_AI.advancedAI");

                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                {
                    //old version: don't caluclate the value of the gems need to the player and the bonus charge (yet)
                    Subdivide_moves_by_color();
                    Enemy_decide_what_to_do();
                }
                else
                {
                    EvalutateMoves();

                    print("big_move_value: " + big_move_value);

                    Search_bonus(true);

                    print("best_bonus_value: " + best_bonus_value);

                    if (best_bonus_value > big_move_value)
                        Enemy_use_bonus();
                    else
                        {
                        Enemy_select_main_gem(enemy_move_selected);
                        Enemy_select_minor_gem();
                        Enemy_move();
                        }
                    }
                break;

            case enemy_AI.by_hand_setup:

                Subdivide_moves_by_color();

                Enemy_decide_what_to_do();

                break;

            case enemy_AI.just_deal_damage:
                Invoke("JustDealDamage", globalRules.enemy_move_delay);
                break;
        }

    }



    void JustDealDamage()
    {
        Damage_opponent(UnityEngine.Random.Range(enemy.myCharacter.justDealDamage_min, enemy.myCharacter.justDealDamage_max));
        Update_turn_order_after_a_bad_move();
    }

    void Enemy_decide_what_to_do()
    {
        print("Enemy_decide_what_to_do");
        if (search_best_move)
        {
            Debug.Log("search best move");
            Enemy_search_best_main_gem();
        }
        else
        {
            print("Enemy_decide_if_use_a_bonus_or_search_the_main_gem");
            Enemy_decide_if_use_a_bonus_or_search_the_main_gem();
        }

        if (enemy_will_use_a_bonus)
        {
            Debug.Log("bonus found, so use bonus");
            Enemy_use_bonus();
        }
        else
        {
            Enemy_select_main_gem(enemy_move_selected);
            Enemy_select_minor_gem();
            Enemy_move();
        }
    }

    void Enemy_search_best_main_gem()
    {
        Debug.Log("Enemy_search_best_main_gem");
        //if there is a bonus and it can be used
        //else..
        if ((!myRuleset.chain_turns_limit) || (enemy.myCharacter.currentChainLenght < myRuleset.max_chain_turns)) //if enemy can gain a turn
        {
            if (myRuleset.gain_turn_if_explode_more_than_3_gems)
            {
                //Debug.Log("enemy try to gain a turn with a big explosion");
                Search_big_explosion();
                if ((enemy_move_selected == -1) && (myRuleset.gain_turn_if_explode_same_color_of_previous_move)) //if no big explosion found, but same color give a turn...
                {
                    //Debug.Log("enemy try to gain a turn using same color of previous move");
                    Search_same_color_of_previous_move();
                }
            }
            else if ((!myRuleset.gain_turn_if_explode_more_than_3_gems) && (myRuleset.gain_turn_if_explode_same_color_of_previous_move))
            {
                //Debug.Log("enemy try to gain a turn using same color of previous move");
                if ((enemy.myCharacter.previous_exploded_color[0] >= 0) || (enemy.myCharacter.previous_exploded_color[1] >= 0))
                    Search_same_color_of_previous_move();
                else
                {
                    Search_big_move();
                }
            }
            else
            {
                Search_big_move();
            }
        }
        else
        {
            Search_big_move();
        }

        //if no move yet found...
        if (enemy_move_selected == -1)
        {
            Enemy_decide_if_use_a_bonus_or_search_the_main_gem();
        }
        else //check if there is a bonus that can do a better result
        {
            if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_bonus)
            {
                temp_enemy_gem_count = new int[myRuleset.gem_length];
                Search_bonus(true);
            }
        }

    }

    void Enemy_decide_if_use_a_bonus_or_search_the_main_gem()
    {
     
        if (UnityEngine.Random.Range(0, 100) <= enemy.myCharacter.chance_of_use_bonus)
        {
            //Debug.Log("use bonus if there is one");
            temp_enemy_gem_count = new int[myRuleset.gem_length];
            Search_bonus(false);
        }

        //if (!enemy_will_use_a_click_bonus_on_board && ! enemy_will_switch_a_gem_to_trigger_a_bonus_on_board)
        if (!enemy_will_use_a_bonus)
        {
            //Debug.Log("no utilizable bonus found, so search main gem");
            Enemy_search_main_gem();
        }
    }



    void Enemy_use_bonus()
    {
        //Debug.Log("enemy use bonus in " + enemy_bonus_click_1_x + "," + enemy_bonus_click_1_y);

        if (myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge || myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
        {
            bonus_select = enemy.myCharacter.bonus_slot[enemy_chosen_bonus_slot];
            if (enemy_chosen_bonus_slot >= 0)
            {
                //Debug.LogWarning("ENEMY USE:  " + enemy_chosen_bonus_slot);
                if (myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge)
                   uIManager.enemyUI.gui_bonus_ico_array[enemy_chosen_bonus_slot].GetComponent<ChargeBonusButton>().EnemyActivate();
                else
                    uIManager.enemyUI.advancedBonusButton[enemy_chosen_bonus_slot].EnemyActivate();

                if (enemy_bonus_click_1_x >= 0)//if this bonus require a click on the board
                {
                    tile_C script_main_gem = script_tiles_array[enemy_bonus_click_1_x, enemy_bonus_click_1_y];
                    script_main_gem.Invoke("Try_to_use_bonus_on_this_tile", globalRules.enemy_move_delay);
                }
            }
        }
        else if (myRuleset.trigger_by_select == Ruleset.trigger_by.inventory)
        {
            //Debug.Log("enemy_chosen_bonus_slot: " + enemy_bonus_inventory_script.bonus_list[enemy_chosen_bonus_slot].name);

            enemy.myUI.bonus_inventory_script.bonus_list[enemy_chosen_bonus_slot].GetComponent<inventory_bonus_button>().EnemyActivate();

            if (enemy_bonus_click_1_x >= 0)//if this bonus require a click on the board
            {
                tile_C script_main_gem = script_tiles_array[enemy_bonus_click_1_x, enemy_bonus_click_1_y];
                script_main_gem.Invoke("Try_to_use_bonus_on_this_tile", globalRules.enemy_move_delay);
            }
        }
        else if ((myRuleset.give_bonus_select == Ruleset.give_bonus.after_big_explosion) || (myRuleset.give_bonus_select == Ruleset.give_bonus.from_stage_file_or_from_gem_emitter))
        {
            if (myRuleset.trigger_by_select == Ruleset.trigger_by.click)
            {
                cursor.position = script_tiles_array[enemy_bonus_click_1_x, enemy_bonus_click_1_y].transform.position;
                tile_C script_main_gem = script_tiles_array[enemy_bonus_click_1_x, enemy_bonus_click_1_y];
                script_main_gem.Invoke("Enemy_click_on_this_bonus", globalRules.enemy_move_delay);
            }
       else if ((myRuleset.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem) || (myRuleset.trigger_by_select == Ruleset.trigger_by.free_switch))
            {
                Debug.Log("switch_a_gem_to_trigger_a_bonus");
                cursor.position = script_tiles_array[enemy_bonus_click_1_x, enemy_bonus_click_1_y].transform.position;

                if (enemy_bonus_click_1_y < enemy_bonus_click_2_y)
                    enemy_move_direction = 4;
                else if (enemy_bonus_click_1_y > enemy_bonus_click_2_y)
                    enemy_move_direction = 5;
                else if (enemy_bonus_click_1_x < enemy_bonus_click_2_x)
                    enemy_move_direction = 6;
                else if (enemy_bonus_click_1_x > enemy_bonus_click_2_x)
                    enemy_move_direction = 7;

                tile_C script_main_gem = script_tiles_array[enemy_bonus_click_1_x, enemy_bonus_click_1_y];
                script_main_gem.I_become_main_gem();

                tile_C script_minor_gem = script_tiles_array[enemy_bonus_click_2_x, enemy_bonus_click_2_y];
                script_minor_gem.I_become_minor_gem(enemy_move_direction);
            }
        }

    }

    void Search_bonus(bool compare_with_big_move_value)
    {
        Debug.Log("Search_bonus");
        //reset variables before use
        bonus_select = Bonus.None;
        enemy_chosen_bonus_slot = -1;
        enemy_bonus_click_1_x = -1;
        enemy_bonus_click_1_y = -1;
        enemy_bonus_click_2_x = -1;
        enemy_bonus_click_2_y = -1;



        if (myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge || myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
            Enemy_check_charge_bonus(compare_with_big_move_value);
        else if ((myRuleset.give_bonus_select == Ruleset.give_bonus.after_big_explosion) || (myRuleset.give_bonus_select == Ruleset.give_bonus.from_stage_file_or_from_gem_emitter))
        {
            if (myRuleset.trigger_by_select == Ruleset.trigger_by.inventory)
                Enemy_check_inventory_bonus(compare_with_big_move_value);
            else
            {
                if (number_of_bonus_on_board > 0)
                {
                    Locate_all_bonus_on_board();
                    if (myRuleset.trigger_by_select == Ruleset.trigger_by.click)
                        Enemy_check_on_board_click_bonus(compare_with_big_move_value);
                    else if (myRuleset.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem)
                        Enemy_check_on_board_switch_bonus(compare_with_big_move_value);
                    else if (myRuleset.trigger_by_select == Ruleset.trigger_by.free_switch)
                        Enemy_check_on_board_free_switch_bonus(compare_with_big_move_value);
                }
            }
        }


    }

    void Enemy_charge_bonus_heal_hp(int my_slot)
    {
        Debug.Log("Enemy_charge_bonus_heal_hp()");
        if (myRuleset.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
        {
            bool already_checked = false;
            if ((enemy.myCharacter.currentHp  + enemy.myCharacter.heal_me_hp_bonus) < enemy.myCharacter.maxHp)//if it is useful heal the enemy now
            {
                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                {
                    already_checked = true;
                    if (player.myCharacter.currentHp > gem_damage_opponent_max_value * 3)//enemy can't win with next move
                    {
                        Enemy_valutate_if_use_heal(my_slot);
                    }
                }
                if (!already_checked)
                    Enemy_valutate_if_use_heal(my_slot);
            }
        }
    }

    void Enemy_valutate_if_use_heal(int my_slot)
    {
        float necessity = 1;

        if (enemy.myCharacter.currentHp  < player.myCharacter.currentHp) //enemy is in disadvantage
            necessity = 1.5f;

        if (enemy.myCharacter.currentHp  < gem_damage_opponent_max_value * 3) //enemy is nearly dead
            necessity = 2;

        if (enemy.myCharacter.heal_me_hp_bonus * necessity > best_bonus_value)
        {
            best_bonus_value = enemy.myCharacter.heal_me_hp_bonus;
            enemy_chosen_bonus_slot = my_slot;
            Debug.Log("enemy use heal");
        }
    }

    void Enemy_charge_bonus_damage_hp(int my_slot)
    {
        Debug.Log("Enemy_charge_bonus_damage_hp()");
        if (myRuleset.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
        {
            bool already_checked = false;

                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                {
                    already_checked = true;
                    Enemy_valutate_if_use_damage(my_slot);
                }
                if (!already_checked)
                    Enemy_valutate_if_use_damage(my_slot);
            
        }
    }

    void Enemy_valutate_if_use_damage(int my_slot)
    {
        float necessity = 1;

        if (player.myCharacter.currentHp <= enemy.myCharacter.damage_opponent_bonus) //final blow
            necessity = 10f;

        if (enemy.myCharacter.damage_opponent_bonus * necessity > best_bonus_value)
        {
            best_bonus_value = (int)(enemy.myCharacter.damage_opponent_bonus * necessity);
            enemy_chosen_bonus_slot = my_slot;
            Debug.Log("enemy chose damage");
        }
    }

    void Enemy_charge_bonus_destroy_all_gem_with_this_color(int i)
    {
        float[] temp_value_for_every_color = new float[myRuleset.gem_length];
        int select_this_color = -1;
        temp_bonus_value = 0;
        temp_enemy_gem_count = new int[myRuleset.gem_length];

        //count how much gems for every color there are on board
        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)) //this tile have something in itself
                {
                    if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                        temp_value_for_every_color[board_array_master[x, y, 1]] += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);
                    else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                    {
                        if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                            temp_value_for_every_color[board_array_master[x, y, 1]]++;
                    }
                    else
                    {
                        if (board_array_master[x, y, 1] < 9)
                            temp_value_for_every_color[board_array_master[x, y, 1]]++;
                    }
                }
            }
        }


        //search the best color
        int random_start_point = UnityEngine.Random.Range(0, myRuleset.gem_length - 1); ;
        for (int g = random_start_point; g < myRuleset.gem_length; g++)
        {
            if (temp_bonus_value < temp_value_for_every_color[g])
            {
                temp_bonus_value = temp_value_for_every_color[g];
                select_this_color = g;
                //Debug.Log("color " + g + " will give: " + temp_value );
            }
        }
        for (int g = 0; g < random_start_point; g++)
        {
            if (temp_bonus_value < temp_value_for_every_color[g])
            {
                temp_bonus_value = temp_value_for_every_color[g];
                select_this_color = g;
                //Debug.Log("(form 0 to random color) color " + g + " will give: " + temp_value );
            }
        }


        //search a tile with that color
        if (temp_bonus_value > best_bonus_value)
        {
            for (int y = 0; y < _Y_tiles; y++)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 1] == select_this_color)
                    {
                        enemy_chosen_bonus_slot = i;
                        enemy_bonus_click_1_x = x;
                        enemy_bonus_click_1_y = y;
                        best_bonus_value = temp_bonus_value;
                    }
                }
            }
        }
    }

    void Enemy_charge_bonus_destroy_horizontal_and_vertical(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
    {
        if (best_bonus_value < max_value_destroy_horizontal_and_vertical)
        {
            //Debug.Log("bonus.destroy_horizontal_and_vertical");
            for (int y = random_start_point_y; y < _Y_tiles; y++)
            {
                for (int x = random_start_point_x; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                    {
                        temp_bonus_value = 0;
                        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                        {
                            temp_bonus_value += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);

                            temp_bonus_value += Calculate_horiz_damage_R(x, y);
                            temp_bonus_value += Calculate_horiz_damage_L(x, y);

                            temp_bonus_value += Calculate_up_damage(x, y);
                            temp_bonus_value += Calculate_down_damage(x, y);
                        }
                        else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                        {
                            temp_enemy_gem_count = new int[myRuleset.gem_length];
                            if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                                temp_bonus_value++;

                            temp_bonus_value += Calculate_horiz_gems_R(x, y);
                            temp_bonus_value += Calculate_horiz_gems_L(x, y);

                            temp_bonus_value += Calculate_up_gems(x, y);
                            temp_bonus_value += Calculate_down_gems(x, y);
                        }
                        else
                        {
                            temp_bonus_value++;
                            temp_bonus_value += R_how_much_gems_will_explode(x, y);
                            temp_bonus_value += L_how_much_gems_will_explode(x, y);

                            temp_bonus_value += Up_how_much_gems_will_explode(x, y);
                            temp_bonus_value += Down_how_much_gems_will_explode(x, y);
                        }


                        if (temp_bonus_value > best_bonus_value)
                        {
                            enemy_chosen_bonus_slot = i;
                            enemy_bonus_click_1_x = x;
                            enemy_bonus_click_1_y = y;
                            best_bonus_value = temp_bonus_value;
                            //Debug.Log("best value = " + best_value + " in " + x + "," + y);

                            if (!search_best_place)
                                return;

                            if (best_bonus_value >= max_value_destroy_vertical) //you can't overcome this outcome, so don't check further
                            {
                                //Debug.Log("vertical and horizontal stop this check at " + x + "," + y);
                                break;
                            }
                        }

                    }
                }
            }
            if (best_bonus_value < max_value_destroy_vertical)
            {
                if (random_start_point_x > 0 || random_start_point_y > 0)
                {
                    //Debug.Log("continue search from 0,0");
                    for (int y = 0; y < random_start_point_y; y++)
                    {
                        for (int x = 0; x < random_start_point_x; x++)
                        {
                            if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                            {
                                temp_bonus_value = 0;
                                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                {
                                    temp_bonus_value += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);

                                    temp_bonus_value += Calculate_horiz_damage_R(x, y);
                                    temp_bonus_value += Calculate_horiz_damage_L(x, y);

                                    temp_bonus_value += Calculate_up_damage(x, y);
                                    temp_bonus_value += Calculate_down_damage(x, y);
                                }
                                else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                {
                                    temp_enemy_gem_count = new int[myRuleset.gem_length];
                                    if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                                        temp_bonus_value++;

                                    temp_bonus_value += Calculate_horiz_gems_R(x, y);
                                    temp_bonus_value += Calculate_horiz_gems_L(x, y);

                                    temp_bonus_value += Calculate_up_gems(x, y);
                                    temp_bonus_value += Calculate_down_gems(x, y);
                                }
                                else
                                {
                                    temp_bonus_value++;
                                    temp_bonus_value += R_how_much_gems_will_explode(x, y);
                                    temp_bonus_value += L_how_much_gems_will_explode(x, y);

                                    temp_bonus_value += Up_how_much_gems_will_explode(x, y);
                                    temp_bonus_value += Down_how_much_gems_will_explode(x, y);
                                }

                                if (temp_bonus_value > best_bonus_value)
                                {
                                    enemy_chosen_bonus_slot = i;
                                    enemy_bonus_click_1_x = x;
                                    enemy_bonus_click_1_y = y;
                                    best_bonus_value = temp_bonus_value;
                                    //Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void Enemy_charge_bonus_destroy_vertical(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
    {
        if (best_bonus_value < max_value_destroy_vertical)
        {
            //Debug.Log("bonus.destroy_vertical");
            for (int y = random_start_point_y; y < _Y_tiles; y++)
            {
                for (int x = random_start_point_x; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                    {
                        temp_bonus_value = 0;
                        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                        {
                            temp_bonus_value += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);

                            temp_bonus_value += Calculate_up_damage(x, y);
                            temp_bonus_value += Calculate_down_damage(x, y);
                        }
                        else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                        {
                            temp_enemy_gem_count = new int[myRuleset.gem_length];
                            if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                                temp_bonus_value++;

                            temp_bonus_value += Calculate_up_gems(x, y);
                            temp_bonus_value += Calculate_down_gems(x, y);
                        }
                        else
                        {
                            temp_bonus_value++;
                            temp_bonus_value += Up_how_much_gems_will_explode(x, y);
                            temp_bonus_value += Down_how_much_gems_will_explode(x, y);
                        }

                        if (temp_bonus_value > best_bonus_value)
                        {
                            enemy_chosen_bonus_slot = i;
                            enemy_bonus_click_1_x = x;
                            enemy_bonus_click_1_y = y;
                            best_bonus_value = temp_bonus_value;
                            //Debug.Log("best value = " + best_value + " in " + x + "," + y);

                            if (!search_best_place)
                                return;

                            if (best_bonus_value >= max_value_destroy_vertical) //you can't overcome this outcome, so don't check further
                            {
                                //Debug.Log("vertical stop this check at " + x + "," + y);
                                break;
                            }
                        }

                    }
                }
            }
            if (best_bonus_value < max_value_destroy_vertical)
            {
                if (random_start_point_x > 0 || random_start_point_y > 0)
                {
                    //	Debug.Log("continue search from 0,0");
                    for (int y = 0; y < random_start_point_y; y++)
                    {
                        for (int x = 0; x < random_start_point_x; x++)
                        {
                            if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                            {
                                temp_bonus_value = 0;
                                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                {
                                    temp_bonus_value += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);

                                    temp_bonus_value += Calculate_up_damage(x, y);
                                    temp_bonus_value += Calculate_down_damage(x, y);
                                }
                                else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                {
                                    temp_enemy_gem_count = new int[myRuleset.gem_length];
                                    if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                                        temp_bonus_value++;

                                    temp_bonus_value += Calculate_up_gems(x, y);
                                    temp_bonus_value += Calculate_down_gems(x, y);
                                }
                                else
                                {
                                    temp_bonus_value++;
                                    temp_bonus_value += Up_how_much_gems_will_explode(x, y);
                                    temp_bonus_value += Down_how_much_gems_will_explode(x, y);
                                }

                                if (temp_bonus_value > best_bonus_value)
                                {
                                    enemy_chosen_bonus_slot = i;
                                    enemy_bonus_click_1_x = x;
                                    enemy_bonus_click_1_y = y;
                                    best_bonus_value = temp_bonus_value;
                                    //Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
                                }
                            }
                        }
                    }
                }
            }
            //else
            //	Debug.Log("vertical skip check from 0,0");
        }
    }

    void Enemy_charge_bonus_destroy_horizontal(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
    {
        print("Enemy_charge_bonus_destroy_horizontal: " + random_start_point_x + "," + random_start_point_y);
        if (best_bonus_value < max_value_destroy_horizontal)
        {
            //Debug.Log("bonus.destroy_horizontal");
            for (int y = random_start_point_y; y < _Y_tiles; y++)
            {
                for (int x = random_start_point_x; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                    {
                        temp_bonus_value = 0;
                        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                        {
                            temp_bonus_value += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);

                            temp_bonus_value += Calculate_horiz_damage_R(x, y);
                            temp_bonus_value += Calculate_horiz_damage_L(x, y);
                        }
                        else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                        {
                            temp_enemy_gem_count = new int[myRuleset.gem_length];
                            if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                                temp_bonus_value++;

                            temp_bonus_value += Calculate_horiz_gems_R(x, y);
                            temp_bonus_value += Calculate_horiz_gems_L(x, y);
                        }
                        else
                        {
                            temp_bonus_value++;
                            temp_bonus_value += R_how_much_gems_will_explode(x, y);
                            temp_bonus_value += L_how_much_gems_will_explode(x, y);
                        }


                        if (temp_bonus_value > best_bonus_value)
                        {
                            enemy_chosen_bonus_slot = i;
                            enemy_bonus_click_1_x = x;
                            enemy_bonus_click_1_y = y;
                            best_bonus_value = temp_bonus_value;
                            //Debug.Log("best value = " + best_value + " in " + x + "," + y);

                            if (!search_best_place)
                                return;

                            if (best_bonus_value >= max_value_destroy_horizontal) //you can't overcome this outcome, so don't check further
                            {
                                //Debug.Log("horizontal stop this check at " + x + "," + y);
                                break;
                            }
                        }
                    }
                }
            }
            if (best_bonus_value < max_value_destroy_horizontal)
            {
                if (random_start_point_x > 0 || random_start_point_y > 0)
                {
                    //Debug.Log("continue search from 0,0");
                    for (int y = 0; y < random_start_point_y; y++)
                    {
                        for (int x = 0; x < random_start_point_x; x++)
                        {
                            if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                            {
                                temp_bonus_value = 0;
                                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                {
                                    temp_bonus_value += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);

                                    temp_bonus_value += Calculate_horiz_damage_R(x, y);
                                    temp_bonus_value += Calculate_horiz_damage_L(x, y);
                                }
                                else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                {
                                    temp_enemy_gem_count = new int[myRuleset.gem_length];
                                    if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                                        temp_bonus_value++;

                                    temp_bonus_value += Calculate_horiz_gems_R(x, y);
                                    temp_bonus_value += Calculate_horiz_gems_L(x, y);
                                }
                                else
                                {
                                    temp_bonus_value++;
                                    temp_bonus_value += R_how_much_gems_will_explode(x, y);
                                    temp_bonus_value += L_how_much_gems_will_explode(x, y);
                                }

                                if (temp_bonus_value > best_bonus_value)
                                {
                                    enemy_chosen_bonus_slot = i;
                                    enemy_bonus_click_1_x = x;
                                    enemy_bonus_click_1_y = y;
                                    best_bonus_value = temp_bonus_value;
                                    //Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
                                }
                            }
                        }
                    }
                }
            }
            //else
            //Debug.Log("horizontal skip check from 0,0");
        }
    }

    void Enemy_charge_bonus_destroy_3x3(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
    {
        if (best_bonus_value < max_value_destroy_3x3) //skip this bonus if it can't overcome the current best value
        {

            for (int y = random_start_point_y; y < _Y_tiles; y++)
            {
                for (int x = random_start_point_x; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                    {
                        temp_bonus_value = 0;

                        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                            temp_bonus_value = Calculate_bomb_damage(x, y);
                        else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                        {
                            temp_enemy_gem_count = new int[myRuleset.gem_length];
                            temp_bonus_value = Calculate_bomb_gems(x, y);
                        }
                        else
                            temp_bonus_value = Bomb_how_much_gems_will_explode(x, y);

                        if (temp_bonus_value > best_bonus_value)
                        {
                            enemy_chosen_bonus_slot = i;
                            enemy_bonus_click_1_x = x;
                            enemy_bonus_click_1_y = y;
                            best_bonus_value = temp_bonus_value;

                            if (!search_best_place)
                                return;

                            //Debug.Log("best value = " + best_value + " in " + x + "," + y);
                            if (best_bonus_value >= max_value_destroy_3x3) //you can't overcome this outcome, so don't check further
                            {
                                //Debug.Log("bomb stop this check at " + x + "," + y);
                                break;
                            }
                        }

                    }
                }
            }
            //search from 0,0 to random point
            if (best_bonus_value < max_value_destroy_3x3)//if you don't have found the best value yet
            {
                if (random_start_point_x > 0 || random_start_point_y > 0)
                {
                    //Debug.Log("continue search from 0,0");
                    for (int y = 0; y < random_start_point_y; y++)
                    {
                        for (int x = 0; x < random_start_point_x; x++)
                        {
                            if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                            {
                                temp_bonus_value = 0;

                                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                    temp_bonus_value = Calculate_bomb_damage(x, y);
                                else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                {
                                    temp_enemy_gem_count = new int[myRuleset.gem_length];
                                    temp_bonus_value = Calculate_bomb_gems(x, y);
                                }
                                else
                                    temp_bonus_value = Bomb_how_much_gems_will_explode(x, y);

                                if (temp_bonus_value > best_bonus_value)
                                {
                                    enemy_chosen_bonus_slot = i;
                                    enemy_bonus_click_1_x = x;
                                    enemy_bonus_click_1_y = y;
                                    best_bonus_value = temp_bonus_value;
                                    //Debug.Log("NEW best value = " + best_value + " in " + x + "," + y);
                                }

                            }
                        }
                    }
                }
            }
            //	else
            //Debug.Log("bomb skip check from 0,0");
        }
    }

    void Enemy_charge_bonus_teleport(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
    {
        if (best_bonus_value < (max_value_switch_gem_teleport_click1 + max_value_switch_gem_teleport_click2))
        {
            //Debug.Log("bonus.switch_gem_teleport");

            float temp_value_teleport_click_1 = 0;
            float temp_best_value_teleport_click_1 = 0;
            int[] teleport_main_color_quantity = new int[myRuleset.gem_length];
            int main_explosion_color = -1;

            //search best click 1
            for (int y = random_start_point_y; y < _Y_tiles; y++)
            {
                for (int x = random_start_point_x; x < _X_tiles; x++)
                {
                    if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)
                        && board_array_master[x, y, 3] == 0) //no padlock
                    {
                        teleport_main_color_quantity = new int[myRuleset.gem_length];

                        if ((y - 1 >= 0) && (board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] < 9))
                        {
                            teleport_main_color_quantity[board_array_master[x, y - 1, 1]]++;
                            if ((y - 2 >= 0) && (board_array_master[x, y - 1, 1] == board_array_master[x, y - 2, 1]))
                                teleport_main_color_quantity[board_array_master[x, y - 1, 1]]++;
                        }

                        if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 1] >= 0) && (board_array_master[x, y + 1, 1] < 9))
                        {
                            teleport_main_color_quantity[board_array_master[x, y + 1, 1]]++;
                            if (((y + 2) < _Y_tiles) && (board_array_master[x, y + 1, 1] == board_array_master[x, y + 2, 1]))
                                teleport_main_color_quantity[board_array_master[x, y + 1, 1]]++;
                        }

                        if ((x - 1 >= 0) && (board_array_master[x - 1, y, 1] >= 0) && (board_array_master[x - 1, y, 1] < 9))
                        {
                            teleport_main_color_quantity[board_array_master[x - 1, y, 1]]++;
                            if ((x - 2 >= 0) && (board_array_master[x - 1, y, 1] == board_array_master[x - 2, y, 1]))
                                teleport_main_color_quantity[board_array_master[x - 1, y, 1]]++;
                        }

                        if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 1] >= 0) && (board_array_master[x + 1, y, 1] < 9))
                        {
                            teleport_main_color_quantity[board_array_master[x + 1, y, 1]]++;
                            if (((x + 2) < _X_tiles) && (board_array_master[x + 1, y, 1] == board_array_master[x + 2, y, 1]))
                                teleport_main_color_quantity[board_array_master[x + 1, y, 1]]++;
                        }


                        for (int c = 0; c < myRuleset.gem_length; c++)
                        {
                            if (teleport_main_color_quantity[c] >= 3) //if this is a big explosion 
                            {
                                //Debug.Log("in " + x +","+y + " *** teleport_main_color_quantity["+c+"] = " + teleport_main_color_quantity[c]);

                                teleport_main_color_quantity[c]++;//count the gem that will be telepor there

                                temp_value_teleport_click_1 = 0;

                                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                    temp_value_teleport_click_1 = (Calculate_damage_of_this_gem(c) * teleport_main_color_quantity[c]);
                                else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                {
                                    temp_enemy_gem_count = new int[myRuleset.gem_length];
                                    for (int cc = 0; cc < teleport_main_color_quantity[c]; cc++)
                                    {
                                        if (Check_if_enemy_need_this_gem_color(c))
                                            temp_value_teleport_click_1++;
                                    }
                                }
                                else
                                    temp_value_teleport_click_1 = teleport_main_color_quantity[c];


                                //Debug.Log("temp_value_teleport_click_1: " + temp_value_teleport_click_1 + " *** temp_best_value_teleport_click_1: " + temp_best_value_teleport_click_1);
                                if (temp_value_teleport_click_1 > temp_best_value_teleport_click_1)
                                {
                                    temp_best_value_teleport_click_1 = temp_value_teleport_click_1;
                                    enemy_bonus_click_1_x = x;
                                    enemy_bonus_click_1_y = y;
                                    main_explosion_color = teleport_main_color_quantity[c];

                                    if (!search_best_place)
                                        break;

                                    if (temp_best_value_teleport_click_1 < max_value_switch_gem_teleport_click1)
                                        break;
                                }

                            }
                        }
                    }
                }
            }

            if (temp_best_value_teleport_click_1 < max_value_switch_gem_teleport_click1)
            {
                for (int y = 0; y < random_start_point_y; y++)
                {
                    for (int x = 0; x < random_start_point_x; x++)
                    {
                        if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)
                            && board_array_master[x, y, 3] == 0) //no padlock
                        {
                            teleport_main_color_quantity = new int[myRuleset.gem_length];


                            if ((y - 1 >= 0) && (board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] < 9))
                            {
                                teleport_main_color_quantity[board_array_master[x, y - 1, 1]]++;
                                if ((y - 2 >= 0) && (board_array_master[x, y - 1, 1] == board_array_master[x, y - 2, 1]))
                                    teleport_main_color_quantity[board_array_master[x, y - 1, 1]]++;
                            }

                            if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 1] >= 0) && (board_array_master[x, y + 1, 1] < 9))
                            {
                                teleport_main_color_quantity[board_array_master[x, y + 1, 1]]++;
                                if (((y + 2) < _Y_tiles) && (board_array_master[x, y + 1, 1] == board_array_master[x, y + 2, 1]))
                                    teleport_main_color_quantity[board_array_master[x, y + 1, 1]]++;
                            }

                            if ((x - 1 >= 0) && (board_array_master[x - 1, y, 1] >= 0) && (board_array_master[x - 1, y, 1] < 9))
                            {
                                teleport_main_color_quantity[board_array_master[x - 1, y, 1]]++;
                                if ((x - 2 >= 0) && (board_array_master[x - 1, y, 1] == board_array_master[x - 2, y, 1]))
                                    teleport_main_color_quantity[board_array_master[x - 1, y, 1]]++;
                            }

                            if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 1] >= 0) && (board_array_master[x + 1, y, 1] < 9))
                            {
                                teleport_main_color_quantity[board_array_master[x + 1, y, 1]]++;
                                if (((x + 2) < _X_tiles) && (board_array_master[x + 1, y, 1] == board_array_master[x + 2, y, 1]))
                                    teleport_main_color_quantity[board_array_master[x + 1, y, 1]]++;
                            }


                            for (int c = 0; c < myRuleset.gem_length; c++)
                            {
                                if (teleport_main_color_quantity[c] >= 3) //if this is a big explosion 
                                {
                                    //Debug.Log("in " + x +","+y + " *** teleport_main_color_quantity["+c+"] = " + teleport_main_color_quantity[c]);

                                    teleport_main_color_quantity[c]++;//count the gem that will be telepor there

                                    temp_value_teleport_click_1 = 0;

                                    if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                        temp_value_teleport_click_1 = (Calculate_damage_of_this_gem(c) * teleport_main_color_quantity[c]);
                                    else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                    {
                                        temp_enemy_gem_count = new int[myRuleset.gem_length];
                                        for (int cc = 0; cc < teleport_main_color_quantity[c]; cc++)
                                        {
                                            if (Check_if_enemy_need_this_gem_color(c))
                                                temp_value_teleport_click_1++;
                                        }
                                    }
                                    else
                                        temp_value_teleport_click_1 = teleport_main_color_quantity[c];


                                    //Debug.Log("temp_value_teleport_click_1: " + temp_value_teleport_click_1 + " *** temp_best_value_teleport_click_1: " + temp_best_value_teleport_click_1);
                                    if (temp_value_teleport_click_1 > temp_best_value_teleport_click_1)
                                    {
                                        temp_best_value_teleport_click_1 = temp_value_teleport_click_1;
                                        enemy_bonus_click_1_x = x;
                                        enemy_bonus_click_1_y = y;
                                        main_explosion_color = teleport_main_color_quantity[c];
                                        //	Debug.Log("teleport click 1: " + enemy_bonus_click_1_x + "," + enemy_bonus_click_1_y + " *** best value = " + temp_best_value_teleport_click_1);

                                        if (!search_best_place)
                                            break;

                                        if (temp_best_value_teleport_click_1 < max_value_switch_gem_teleport_click1)
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }



            //search best minor explosion IF the minor color not is absorb or repell for the player
            if (temp_best_value_teleport_click_1 > 0)
            {
                //Debug.Log("search secondary gem for teleport");
                float temp_value_teleport_click_2 = 0;
                float temp_best_value_teleport_click_2 = 0;

                //exclude the gems target from this seach
                bool[,] reserved_gem = new bool[_X_tiles, _Y_tiles];
                if (enemy_bonus_click_1_y - 1 >= 0)
                {
                    reserved_gem[enemy_bonus_click_1_x, enemy_bonus_click_1_y - 1] = true;

                    if ((enemy_bonus_click_1_y - 2 >= 0) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y - 2, 1] == main_explosion_color))
                        reserved_gem[enemy_bonus_click_1_x, enemy_bonus_click_1_y - 2] = true;

                }


                if ((enemy_bonus_click_1_y + 1) < _Y_tiles)
                {
                    reserved_gem[enemy_bonus_click_1_x, enemy_bonus_click_1_y + 1] = true;

                    if (((enemy_bonus_click_1_y + 2) < _Y_tiles) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y + 2, 1] == main_explosion_color))
                        reserved_gem[enemy_bonus_click_1_x, enemy_bonus_click_1_y + 2] = true;
                }

                if (enemy_bonus_click_1_x - 1 >= 0)
                {
                    reserved_gem[enemy_bonus_click_1_x - 1, enemy_bonus_click_1_y] = true;

                    if ((enemy_bonus_click_1_x - 2 >= 0) && (board_array_master[enemy_bonus_click_1_x - 2, enemy_bonus_click_1_y, 1] == main_explosion_color))
                        reserved_gem[enemy_bonus_click_1_x - 2, enemy_bonus_click_1_y] = true;
                }


                if ((enemy_bonus_click_1_x + 1) < _X_tiles)
                {
                    reserved_gem[enemy_bonus_click_1_x + 1, enemy_bonus_click_1_y] = true;

                    if (((enemy_bonus_click_1_x + 2) < _X_tiles) && (board_array_master[enemy_bonus_click_1_x + 2, enemy_bonus_click_1_y, 1] == main_explosion_color))
                        reserved_gem[enemy_bonus_click_1_x + 2, enemy_bonus_click_1_y] = true;
                }

                bool secondary_color_must_explode = true;
                if ((myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                    && Calculate_damage_of_this_gem(board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]) <= 0)
                    secondary_color_must_explode = false;

                if (secondary_color_must_explode)
                {

                    //Debug.Log("the secondary color must explode");
                    int minor_color_quantity = 0;

                    for (int y = random_start_point_y; y < _Y_tiles; y++)
                    {
                        for (int x = random_start_point_x; x < _X_tiles; x++)
                        {
                            if (board_array_master[x, y, 1] == main_explosion_color)//if this gem have the same color of the explosion that I want trigger
                            {
                                if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)
                                    && board_array_master[x, y, 3] == 0 //no padlock
                                    && !reserved_gem[x, y])//no reserved gem
                                {

                                    minor_color_quantity = 0;

                                    if ((y - 1 >= 0) && (board_array_master[x, y - 1, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if ((y - 2 >= 0) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x, y - 2, 1]))
                                            minor_color_quantity++;
                                    }

                                    if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if (((y + 2) < _Y_tiles) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x, y + 2, 1]))
                                            minor_color_quantity++;
                                    }

                                    if ((x - 1 >= 0) && (board_array_master[x - 1, y, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if ((x - 2 >= 0) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x - 2, y, 1]))
                                            minor_color_quantity++;
                                    }

                                    if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if (((x + 2) < _X_tiles) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x + 2, y, 1]))
                                            minor_color_quantity++;
                                    }


                                    temp_value_teleport_click_2 = 0;

                                    if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                        temp_value_teleport_click_2 = (Calculate_damage_of_this_gem(board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]) * minor_color_quantity);
                                    else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                    {
                                        temp_enemy_gem_count = new int[myRuleset.gem_length];
                                        for (int cc = 0; cc < minor_color_quantity; cc++)
                                        {
                                            if (Check_if_enemy_need_this_gem_color(board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                                temp_value_teleport_click_2++;
                                        }
                                    }
                                    else
                                        temp_value_teleport_click_2 = minor_color_quantity;

                                    //check if this is the best choice
                                    if (temp_value_teleport_click_2 > temp_best_value_teleport_click_2)
                                    {
                                        enemy_chosen_bonus_slot = i;
                                        enemy_bonus_click_2_x = x;
                                        enemy_bonus_click_2_y = y;
                                        temp_best_value_teleport_click_2 = temp_value_teleport_click_2;

                                        //Debug.Log("teleport click 2: " + enemy_bonus_click_2_x + "," + enemy_bonus_click_2_y + " *** best value = " + temp_best_value_teleport_click_2);

                                        if (!search_best_place)
                                            break;

                                        if (temp_best_value_teleport_click_2 >= max_value_switch_gem_teleport_click2)
                                            break;
                                    }

                                }
                            }
                        }
                    }
                    if (temp_best_value_teleport_click_2 < max_value_switch_gem_teleport_click2)
                    {
                        for (int y = 0; y < random_start_point_y; y++)
                        {
                            for (int x = 0; x < random_start_point_x; x++)
                            {
                                if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)
                                    && board_array_master[x, y, 3] == 0 //no padlock
                                    && !reserved_gem[x, y])//no reserved gem
                                {
                                    minor_color_quantity = 0;

                                    if ((y - 1 >= 0) && (board_array_master[x, y - 1, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if ((y - 2 >= 0) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x, y - 2, 1]))
                                            minor_color_quantity++;
                                    }

                                    if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if (((y + 2) < _Y_tiles) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x, y + 2, 1]))
                                            minor_color_quantity++;
                                    }

                                    if ((x - 1 >= 0) && (board_array_master[x - 1, y, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if ((x - 2 >= 0) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x - 2, y, 1]))
                                            minor_color_quantity++;
                                    }

                                    if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 1] == board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                    {
                                        minor_color_quantity++;
                                        if (((x + 2) < _X_tiles) && (board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1] == board_array_master[x + 2, y, 1]))
                                            minor_color_quantity++;
                                    }


                                    temp_value_teleport_click_2 = 0;

                                    if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                        temp_value_teleport_click_2 = (Calculate_damage_of_this_gem(board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]) * minor_color_quantity);
                                    else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                    {
                                        temp_enemy_gem_count = new int[myRuleset.gem_length];
                                        for (int cc = 0; cc < minor_color_quantity; cc++)
                                        {
                                            if (Check_if_enemy_need_this_gem_color(board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]))
                                                temp_value_teleport_click_2++;
                                        }
                                    }
                                    else
                                        temp_value_teleport_click_2 = minor_color_quantity;

                                    //check if this is the best choice
                                    if (temp_value_teleport_click_2 > temp_best_value_teleport_click_2)
                                    {
                                        enemy_chosen_bonus_slot = i;
                                        enemy_bonus_click_2_x = x;
                                        enemy_bonus_click_2_y = y;
                                        temp_best_value_teleport_click_2 = temp_value_teleport_click_2;

                                        //Debug.Log("teleport click 2: " + enemy_bonus_click_2_x + "," + enemy_bonus_click_2_y + " *** best value = " + temp_best_value_teleport_click_2);

                                        if (!search_best_place)
                                            break;

                                        if (temp_best_value_teleport_click_2 >= max_value_switch_gem_teleport_click2)
                                            break;
                                    }

                                }
                            }
                        }
                    }



                }
                else //the secondary color don't must explode!
                {
                    //Debug.Log("the secondary color don't must explode");
                    for (int y = random_start_point_y; y < _Y_tiles; y++)
                    {
                        for (int x = random_start_point_x; x < _X_tiles; x++)
                        {
                            if (Count_how_much_gems_with_this_color_there_are_around_this_coordinates(x, y, board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]) == 0)
                            {
                                if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)
                                    && board_array_master[x, y, 3] == 0 //no padlock
                                    && !reserved_gem[x, y])//no reserved gem
                                {
                                    enemy_chosen_bonus_slot = i;
                                    enemy_bonus_click_2_x = x;
                                    enemy_bonus_click_2_y = y;
                                    best_bonus_value = temp_best_value_teleport_click_1;
                                    break;
                                }
                            }
                        }
                    }
                    if (enemy_bonus_click_2_x == -1)
                    {
                        for (int y = 0; y < random_start_point_y; y++)
                        {
                            for (int x = 0; x < random_start_point_x; x++)
                            {
                                if (Count_how_much_gems_with_this_color_there_are_around_this_coordinates(x, y, board_array_master[enemy_bonus_click_1_x, enemy_bonus_click_1_y, 1]) == 0)
                                {
                                    if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)
                                        && board_array_master[x, y, 3] == 0 //no padlock
                                        && !reserved_gem[x, y])//no reserved gem
                                    {
                                        enemy_chosen_bonus_slot = i;
                                        enemy_bonus_click_2_x = x;
                                        enemy_bonus_click_2_y = y;
                                        best_bonus_value = temp_best_value_teleport_click_1;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
                if (enemy_bonus_click_2_x >= 0)
                {
                    if ((temp_best_value_teleport_click_1 + temp_best_value_teleport_click_2) > best_bonus_value)
                        best_bonus_value = temp_best_value_teleport_click_1 + temp_best_value_teleport_click_2;
                }
                //else
                //Debug.Log("click_2 not found");

            }

        }
    }

    void Enemy_charge_bonus_destroy_one(int random_start_point_x, int random_start_point_y, int i, bool search_best_place)
    {
        if (best_bonus_value < max_value_destroy_one)
        {
            //Debug.Log("bonus.destroy_one");

            int use_the_bonus_here_will_make_explode_n_gems = 0;

            for (int y = random_start_point_y; y < _Y_tiles; y++)
            {
                for (int x = random_start_point_x; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                    {
                        if (((y - 2) >= 0) && ((y + 2) < _Y_tiles))
                        {

                            if ((board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] < 9) //there is a gem over me
                                && (board_array_master[x, y - 1, 1] == board_array_master[x, y - 2, 1]) //two gem over me
                                && (board_array_master[x, y - 1, 1] == board_array_master[x, y + 1, 1]) //one under
                                && (board_array_master[x, y - 1, 1] == board_array_master[x, y + 2, 1]) //another under
                                ) //then use bonus here will explode at least 4 gems
                            {
                                use_the_bonus_here_will_make_explode_n_gems = 4;
                                //search for lateral gems
                                if (((x - 1) >= 0) && ((x + 1) < _X_tiles))
                                {
                                    if ((board_array_master[x, y - 1, 1] == board_array_master[x + 1, y, 1])
                                        && (board_array_master[x, y - 1, 1] == board_array_master[x - 1, y, 1]))
                                    {
                                        use_the_bonus_here_will_make_explode_n_gems += 2;
                                        if (((x - 2) >= 0) && (board_array_master[x, y - 1, 1] == board_array_master[x - 2, y, 1]))
                                            use_the_bonus_here_will_make_explode_n_gems++;
                                        if (((x + 2) < _X_tiles) && (board_array_master[x, y - 1, 1] == board_array_master[x + 2, y, 1]))
                                            use_the_bonus_here_will_make_explode_n_gems++;
                                    }
                                }
                                else if ((x - 2) >= 0)
                                {
                                    if ((board_array_master[x, y - 1, 1] == board_array_master[x - 1, y, 1])
                                        && (board_array_master[x, y - 1, 1] == board_array_master[x - 2, y, 1]))
                                        use_the_bonus_here_will_make_explode_n_gems += 2;
                                }
                                else if ((x + 2) < _X_tiles)
                                {
                                    if ((board_array_master[x, y - 1, 1] == board_array_master[x + 1, y, 1])
                                        && (board_array_master[x, y - 1, 1] == board_array_master[x + 2, y, 1]))
                                        use_the_bonus_here_will_make_explode_n_gems += 2;
                                }

                                //calculate the value of this move:
                                temp_bonus_value = 0;

                                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                    temp_bonus_value = Calculate_damage_of_this_gem(board_array_master[x, y - 1, 1]) * use_the_bonus_here_will_make_explode_n_gems;
                                else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                {
                                    temp_enemy_gem_count = new int[myRuleset.gem_length];
                                    for (int c = 0; c < use_the_bonus_here_will_make_explode_n_gems; c++)
                                    {
                                        if (Check_if_enemy_need_this_gem_color(board_array_master[x, y - 1, 1]))
                                            temp_bonus_value++;
                                    }
                                }
                                else
                                    temp_bonus_value = use_the_bonus_here_will_make_explode_n_gems;

                                if (temp_bonus_value > best_bonus_value)
                                {
                                    enemy_chosen_bonus_slot = i;
                                    enemy_bonus_click_1_x = x;
                                    enemy_bonus_click_1_y = y;
                                    best_bonus_value = temp_bonus_value;
                                    //Debug.Log("best value = " + best_value + " in " + x + "," + y);

                                    if (!search_best_place)
                                        return;

                                    if (best_bonus_value >= max_value_destroy_one) //you can't overcome this outcome, so don't check further
                                    {
                                        //Debug.Log("hammer stop this check at " + x + "," + y);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (best_bonus_value < max_value_destroy_one)//if you don't have found the best value yet
            {
                if (random_start_point_x > 0 || random_start_point_y > 0)
                {
                    //Debug.Log("continue search from 0,0");
                    for (int y = 0; y < random_start_point_y; y++)
                    {
                        for (int x = 0; x < random_start_point_x; x++)
                        {
                            if (board_array_master[x, y, 1] >= 0) //this tile have something in itself
                            {
                                if (((y - 2) >= 0) && ((y + 2) < _Y_tiles))
                                {

                                    if ((board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] < 9) //there is a gem over me
                                        && (board_array_master[x, y - 1, 1] == board_array_master[x, y - 2, 1]) //two gem over me
                                        && (board_array_master[x, y - 1, 1] == board_array_master[x, y + 1, 1]) //one under
                                        && (board_array_master[x, y - 1, 1] == board_array_master[x, y + 2, 1]) //another under
                                        ) //then use bonus here will explode at least 4 gems
                                    {
                                        use_the_bonus_here_will_make_explode_n_gems = 4;
                                        //search for lateral gems
                                        if (((x - 1) >= 0) && ((x + 1) < _X_tiles))
                                        {
                                            if ((board_array_master[x, y - 1, 1] == board_array_master[x + 1, y, 1])
                                                && (board_array_master[x, y - 1, 1] == board_array_master[x - 1, y, 1]))
                                            {
                                                use_the_bonus_here_will_make_explode_n_gems += 2;
                                                if (((x - 2) >= 0) && (board_array_master[x, y - 1, 1] == board_array_master[x - 2, y, 1]))
                                                    use_the_bonus_here_will_make_explode_n_gems++;
                                                if (((x + 2) < _X_tiles) && (board_array_master[x, y - 1, 1] == board_array_master[x + 2, y, 1]))
                                                    use_the_bonus_here_will_make_explode_n_gems++;
                                            }
                                        }
                                        else if ((x - 2) >= 0)
                                        {
                                            if ((board_array_master[x, y - 1, 1] == board_array_master[x - 1, y, 1])
                                                && (board_array_master[x, y - 1, 1] == board_array_master[x - 2, y, 1]))
                                                use_the_bonus_here_will_make_explode_n_gems += 2;
                                        }
                                        else if ((x + 2) < _X_tiles)
                                        {
                                            if ((board_array_master[x, y - 1, 1] == board_array_master[x + 1, y, 1])
                                                && (board_array_master[x, y - 1, 1] == board_array_master[x + 2, y, 1]))
                                                use_the_bonus_here_will_make_explode_n_gems += 2;
                                        }

                                        //calculate the value of this move:
                                        temp_bonus_value = 0;

                                        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                                            temp_bonus_value = Calculate_damage_of_this_gem(board_array_master[x, y - 1, 1]) * use_the_bonus_here_will_make_explode_n_gems;
                                        else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                                        {
                                            temp_enemy_gem_count = new int[myRuleset.gem_length];
                                            for (int c = 0; c < use_the_bonus_here_will_make_explode_n_gems; c++)
                                            {
                                                if (Check_if_enemy_need_this_gem_color(board_array_master[x, y - 1, 1]))
                                                    temp_bonus_value++;
                                            }
                                        }
                                        else
                                            temp_bonus_value = use_the_bonus_here_will_make_explode_n_gems;

                                        if (temp_bonus_value > best_bonus_value)
                                        {
                                            enemy_chosen_bonus_slot = i;
                                            enemy_bonus_click_1_x = x;
                                            enemy_bonus_click_1_y = y;
                                            best_bonus_value = temp_bonus_value;
                                            //	Debug.Log("best value = " + best_value + " in " + x + "," + y);
                                            if (best_bonus_value >= max_value_destroy_one) //you can't overcome this outcome, so don't check further
                                            {
                                                //Debug.Log("hammer stop this check at " + x + "," + y);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }



        }
    }



    void Enemy_check_inventory_bonus(bool compare_with_big_move_value)
    {
        temp_bonus_value = 0;
        best_bonus_value = 0;

        enemy_will_use_a_bonus = false;

        int random_start_point_x = UnityEngine.Random.Range(0, _X_tiles - 1);
        int random_start_point_y = UnityEngine.Random.Range(0, _Y_tiles - 1);

        if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_best_bonus) //search the most strong bonus avaible
        {
            for (int i = 1; i < Enum.GetNames(typeof(Bonus)).Length; i++)
            {
                if (enemy.myCharacter.bonus_inventory[i] > 0)
                    Enemy_try_inventory_bonus(random_start_point_x, random_start_point_y, i, true);
            }
        }
        else //pick a random bonus
        {
            //select a random bonus slot
            int random_start_slot = UnityEngine.Random.Range(0, Enum.GetNames(typeof(Bonus)).Length - 1);
            int select_this_slot = -1;
            for (int i = random_start_slot; i < Enum.GetNames(typeof(Bonus)).Length; i++)
            {
                if (enemy.myCharacter.bonus_inventory[i] > 0)//if there are bonuses that can be used
                {
                    select_this_slot = i;
                    break;
                }
            }
            if (select_this_slot == -1)
            {
                for (int i = 0; i < random_start_slot; i++)
                {
                    if (enemy.myCharacter.bonus_inventory[i] > 0)//if there are bonuses that can be used
                    {
                        select_this_slot = i;
                        break;
                    }
                }
            }


            if (select_this_slot > 0)//you have found a bonus ready to use, now select a random place where use it
            {
                Enemy_try_inventory_bonus(random_start_point_x, random_start_point_y, select_this_slot, false);
            }
        }

        if (best_bonus_value > 0)
            enemy_will_use_a_bonus = true;
    }

    void Enemy_check_charge_bonus(bool compare_with_big_move_value)
    {
        temp_bonus_value = 0;
        best_bonus_value = 0;

        enemy_will_use_a_bonus = false;

        int random_start_point_x = UnityEngine.Random.Range(0, _X_tiles - 1);
        int random_start_point_y = UnityEngine.Random.Range(0, _Y_tiles - 1);

        if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_best_bonus) //search the most strong bonus avaible
        {
            for (int i = 0; i < enemy.myCharacter.bonus_slot_availables; i++)
            {
                if (enemy.myCharacter.bonus_ready[i])//if there are bonuses that can be used
                {
                    Enemy_try_charge_bonus(random_start_point_x, random_start_point_y, i, true);
                }
            }
        }
        else//pick out a random avaible bonus
        {

            //select a random bonus slot
            int random_start_slot = UnityEngine.Random.Range(0, enemy.myCharacter.bonus_slot_availables - 1);
            int select_this_slot = -1;
            for (int i = random_start_slot; i < enemy.myCharacter.bonus_slot_availables; i++)
            {
                if (enemy.myCharacter.bonus_ready[i])//if there are bonuses that can be used
                {
                    select_this_slot = i;
                    break;
                }
            }
            if (select_this_slot == -1)
            {
                for (int i = 0; i < random_start_slot; i++)
                {
                    if (enemy.myCharacter.bonus_ready[i])//if there are bonuses that can be used
                    {
                        select_this_slot = i;
                        break;
                    }
                }
            }


            if (select_this_slot >= 0)//you have found a bonus ready to use, now select a random place where use it
            {
                print("B");
                Enemy_try_charge_bonus(random_start_point_x, random_start_point_y, select_this_slot, false);
            }
        }

        //print("Enemy_check_charge_bonus... best_value: " + best_value);
        if (best_bonus_value > 0)
            enemy_will_use_a_bonus = true;

    }

    void Enemy_try_charge_bonus(int random_start_point_x, int random_start_point_y, int selected_slot, bool search_best_place)
    {

        Bonus tempBonus = enemy.myCharacter.bonus_slot[selected_slot];

        if (myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
            tempBonus = enemy.myCharacter.advancedChargeBonuses[selected_slot].myBonus;

        //print("Enemy_try_charge_bonus(" + random_start_point_x + "," + random_start_point_y + "," + selected_slot + "," + search_best_place + " = " + tempBonus);

        switch (tempBonus)//search best place on the board, where use this bonus
        {
            case Bonus.DestroyOne:
                Enemy_charge_bonus_destroy_one(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
                break;

            case Bonus.SwitchGemTeleport:
                Enemy_charge_bonus_teleport(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
                break;

            case Bonus.Destroy3x3:
                Enemy_charge_bonus_destroy_3x3(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
                break;

            case Bonus.DestroyHorizontal:
                Enemy_charge_bonus_destroy_horizontal(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
                break;

            case Bonus.DestroyVertical:
                Enemy_charge_bonus_destroy_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
                break;

            case Bonus.DestroyHorizontalAndVertical:
                Enemy_charge_bonus_destroy_horizontal_and_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
                break;

            case Bonus.DestroyAllGemsWithThisColor:
                Enemy_charge_bonus_destroy_all_gem_with_this_color(selected_slot);
                break;

            case Bonus.HealMe:
                Enemy_charge_bonus_heal_hp(selected_slot);
                break;

            case Bonus.DamageOpponent:
                Enemy_charge_bonus_damage_hp(selected_slot);
                break;
        }
    }

    void Enemy_try_inventory_bonus(int random_start_point_x, int random_start_point_y, int selected_slot, bool search_best_place)
    {
        Debug.Log("_____Enemy_try_inventory_bonus: " + selected_slot.ToString());

        if (selected_slot == 1)
            Enemy_charge_bonus_destroy_one(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
        else if (selected_slot == 2)
            Enemy_charge_bonus_teleport(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
        else if (selected_slot == 3)
            Enemy_charge_bonus_destroy_3x3(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
        else if (selected_slot == 4)
            Enemy_charge_bonus_destroy_horizontal(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
        else if (selected_slot == 5)
            Enemy_charge_bonus_destroy_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
        else if (selected_slot == 6)
            Enemy_charge_bonus_destroy_horizontal_and_vertical(random_start_point_x, random_start_point_y, selected_slot, search_best_place);
        else if (selected_slot == 7)
            Enemy_charge_bonus_destroy_all_gem_with_this_color(selected_slot);
        else if (selected_slot == 10)
            Enemy_charge_bonus_heal_hp(selected_slot);
        else if (selected_slot == 11)
            Enemy_charge_bonus_damage_hp(selected_slot);

    }

    void Locate_all_bonus_on_board()
    {
        Debug.Log("Locate_all_bonus_on_board()");
        bonus_coordinate = new Vector2[number_of_bonus_on_board];
        int bonus_count = 0;
        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (board_array_master[x, y, 4] > 0)//if this is a bonus
                {
                    bonus_coordinate[bonus_count] = new Vector2(x, y);
                    bonus_count++;
                }
            }
        }
    }

    void Check_if_I_can_use_this_switch_bonus(int i)
    {
        //up gem can go down over me?
        if (((int)bonus_coordinate[i].y > 0) && (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y - 1, 6] > 0))
            Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, 0, -1);

        //down gem can go up over me?
        if (((int)bonus_coordinate[i].y + 1 < _Y_tiles) && (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y + 1, 7] > 0))
            Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, 0, 1);

        //R gem can go L over me?
        if (((int)bonus_coordinate[i].x + 1 < _X_tiles) && (board_array_master[(int)bonus_coordinate[i].x + 1, (int)bonus_coordinate[i].y, 9] > 0))
            Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, 1, 0);

        //L gem can go R over me?
        if (((int)bonus_coordinate[i].x > 0) && (board_array_master[(int)bonus_coordinate[i].x - 1, (int)bonus_coordinate[i].y, 8] > 0))
            Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, -1, 0);

    }

    void Check_if_I_can_use_this_free_switch_bonus(int i)
    {
        //up gem can go down over me?
        if (((int)bonus_coordinate[i].y > 0)
            && (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y - 1, 1] >= 0) && (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y - 1, 1] <= 9) //is a gem or a special gem
            )
        {
            if (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y - 1, 3] == 0) //no padlock
                Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, 0, -1);
        }

        //down gem can go up over me?
        if (((int)bonus_coordinate[i].y + 1 < _Y_tiles)
            && (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y + 1, 1] >= 0) && (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y + 1, 1] <= 9))
        {
            if (board_array_master[(int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y + 1, 3] == 0)
                Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, 0, 1);
        }

        //R gem can go L over me?
        if (((int)bonus_coordinate[i].x + 1 < _X_tiles)
            && (board_array_master[(int)bonus_coordinate[i].x + 1, (int)bonus_coordinate[i].y, 1] >= 0) && (board_array_master[(int)bonus_coordinate[i].x + 1, (int)bonus_coordinate[i].y, 1] <= 9))
        {
            if (board_array_master[(int)bonus_coordinate[i].x + 1, (int)bonus_coordinate[i].y, 3] == 0)
                Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, 1, 0);
        }

        //L gem can go R over me?
        if (((int)bonus_coordinate[i].x > 0)
            && (board_array_master[(int)bonus_coordinate[i].x - 1, (int)bonus_coordinate[i].y, 1] >= 0) && (board_array_master[(int)bonus_coordinate[i].x - 1, (int)bonus_coordinate[i].y, 1] <= 9))
        {
            if (board_array_master[(int)bonus_coordinate[i].x - 1, (int)bonus_coordinate[i].y, 3] == 0)
                Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, -1, 0);
        }
    }

    void Enemy_check_on_board_free_switch_bonus(bool compare_with_big_move_value)
    {
        //Debug.LogWarning("Enemy_check_on_board_free_switch_bonus: " + number_of_bonus_on_board);
        temp_bonus_value = 0;
        best_bonus_value = 0;

        if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_best_bonus) //search the most useful bonus
        {
            Debug.Log("Enemy_check_on_board_switch_bonus: Best");

            for (int i = 0; i < number_of_bonus_on_board; i++)
                Check_if_I_can_use_this_free_switch_bonus(i);

            if (compare_with_big_move_value)
            {
                if (best_bonus_value > big_move_value)
                    enemy_will_use_a_bonus = true;
                //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
            }
            else
            {
                if (best_bonus_value > 0)
                    enemy_will_use_a_bonus = true;
                //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
            }
        }
        else //select a random bonus
        {
            Debug.Log("Enemy_check_on_board_switch_bonus: Random");

            if (number_of_bonus_on_board == 1)
            {
                Check_if_I_can_use_this_free_switch_bonus(0);
                if (best_bonus_value > 0)
                    enemy_will_use_a_bonus = true;
                //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
            }
            else //start check bonus from a random start point and stop to first bonus usable
            {
                int random_start_point = UnityEngine.Random.Range(0, number_of_bonus_on_board - 1);

                for (int i = random_start_point; i < number_of_bonus_on_board; i++)
                {

                    Check_if_I_can_use_this_free_switch_bonus(i);

                    if (best_bonus_value > 0)
                    {
                        enemy_will_use_a_bonus = true;
                        //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
                        return;
                    }
                }
                //if you don't have found an usable bonus yet, check from 0 start point to random start point
                if (best_bonus_value == 0)
                {
                    for (int i = 0; i < random_start_point; i++)
                    {
                        Check_if_I_can_use_this_free_switch_bonus(i);

                        if (best_bonus_value > 0)
                        {
                            //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
                            enemy_will_use_a_bonus = true;
                            return;
                        }
                    }
                }
            }
        }
    }

    void Enemy_check_on_board_switch_bonus(bool compare_with_big_move_value)
    {
        Debug.LogWarning("Enemy_check_on_board_switch_bonus: " + number_of_bonus_on_board);
        temp_bonus_value = 0;
        best_bonus_value = 0;

        if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_best_bonus) //search the most useful bonus
        {
            Debug.Log("Enemy_check_on_board_switch_bonus: Best");

            for (int i = 0; i < number_of_bonus_on_board; i++)
                Check_if_I_can_use_this_switch_bonus(i);

            if (compare_with_big_move_value)
            {
                if (best_bonus_value > big_move_value)
                    enemy_will_use_a_bonus = true;
                //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
            }
            else
            {
                if (best_bonus_value > 0)
                    enemy_will_use_a_bonus = true;
                //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
            }
        }
        else //select a random bonus
        {
            Debug.Log("Enemy_check_on_board_switch_bonus: Random");

            if (number_of_bonus_on_board == 1)
            {
                Check_if_I_can_use_this_switch_bonus(0);
                if (best_bonus_value > 0)
                    enemy_will_use_a_bonus = true;
                //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
            }
            else //start check bonus from a random start point and stop to first bonus usable
            {
                int random_start_point = UnityEngine.Random.Range(0, number_of_bonus_on_board - 1);

                for (int i = random_start_point; i < number_of_bonus_on_board; i++)
                {

                    Check_if_I_can_use_this_switch_bonus(i);

                    if (best_bonus_value > 0)
                    {
                        enemy_will_use_a_bonus = true;
                        //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
                        return;
                    }
                }
                //if you don't have found an usable bonus yet, check from 0 start point to random start point
                if (best_bonus_value == 0)
                {
                    for (int i = 0; i < random_start_point; i++)
                    {
                        Check_if_I_can_use_this_switch_bonus(i);

                        if (best_bonus_value > 0)
                        {
                            //enemy_will_switch_a_gem_to_trigger_a_bonus_on_board = true;
                            enemy_will_use_a_bonus = true;
                            return;
                        }
                    }
                }
            }
        }

    }

    void Search_the_most_useful_bonus_on_board(int x, int y, int x_correction, int y_correction)
    {
        temp_bonus_value = Enemy_check_value_of_this_bonus_on_board(x, y, 0, 0);

        if (temp_bonus_value > best_bonus_value)
        {
            enemy_bonus_click_1_x = x + x_correction;
            enemy_bonus_click_1_y = y + y_correction;

            enemy_bonus_click_2_x = x;
            enemy_bonus_click_2_y = y;

            best_bonus_value = temp_bonus_value;
        }
        else if (temp_bonus_value == best_bonus_value)
        {
            if (UnityEngine.Random.Range(1, 100) > UnityEngine.Random.Range(40, 60))
            {
                enemy_bonus_click_1_x = x + x_correction;
                enemy_bonus_click_1_y = y + y_correction;

                enemy_bonus_click_2_x = x;
                enemy_bonus_click_2_y = y;

                best_bonus_value = temp_bonus_value;
            }
        }
    }

    void Enemy_check_on_board_click_bonus(bool compare_with_big_move_value)
    {

        if (UnityEngine.Random.Range(1, 100) <= enemy.myCharacter.chance_of_use_best_bonus)
        {
            //search the most useful bonus
            temp_bonus_value = 0;
            best_bonus_value = 0;
            for (int i = 0; i < number_of_bonus_on_board; i++)
            {
                Search_the_most_useful_bonus_on_board((int)bonus_coordinate[i].x, (int)bonus_coordinate[i].y, 0, 0);
            }

            if (compare_with_big_move_value)
            {
                if (best_bonus_value > big_move_value)
                    enemy_will_use_a_bonus = true;
                //enemy_will_use_a_click_bonus_on_board = true;
            }
            else
            {
                if (best_bonus_value > 0)
                    enemy_will_use_a_bonus = true;
                //enemy_will_use_a_click_bonus_on_board = true;
            }
        }
        else //select a random bonus
        {
            int random_bonus_selected = 0;
            random_bonus_selected = UnityEngine.Random.Range(0, bonus_coordinate.Length - 1);
            enemy_bonus_click_1_x = (int)bonus_coordinate[random_bonus_selected].x;
            enemy_bonus_click_1_y = (int)bonus_coordinate[random_bonus_selected].y;

            if (compare_with_big_move_value)
            {
                if (Enemy_check_value_of_this_bonus_on_board(enemy_bonus_click_1_x, enemy_bonus_click_1_y, 0, 0) > big_move_value)
                    enemy_will_use_a_bonus = true;
                //enemy_will_use_a_click_bonus_on_board = true;
            }
            else
                enemy_will_use_a_bonus = true;
            //enemy_will_use_a_click_bonus_on_board = true;
        }

    }

    float Enemy_check_value_of_this_bonus_on_board(int x, int y, int x_correction, int y_correction)
    {
        float temp_value = 0;

        if (board_array_master[x, y, 4] == 3)//bomb
        {
            if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
                temp_value = Calculate_bomb_damage(x + x_correction, y + y_correction);
            else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                temp_value = Calculate_bomb_gems(x + x_correction, y + y_correction);
            else
                temp_value = Bomb_how_much_gems_will_explode(x + x_correction, y + y_correction);
        }
        else if (board_array_master[x, y, 4] == 4)//horiz
        {
            if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
            {
                temp_value += Calculate_horiz_damage_R(x + x_correction, y + y_correction);
                temp_value += Calculate_horiz_damage_L(x + x_correction, y + y_correction);
            }
            else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
            {
                temp_value += Calculate_horiz_gems_R(x + x_correction, y + y_correction);
                temp_value += Calculate_horiz_gems_L(x + x_correction, y + y_correction);
            }
            else
            {
                temp_value += R_how_much_gems_will_explode(x + x_correction, y + y_correction);
                temp_value += L_how_much_gems_will_explode(x + x_correction, y + y_correction);
            }
        }
        else if (board_array_master[x, y, 4] == 5)//vertic
        {
            if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
            {
                temp_value += Calculate_up_damage(x + x_correction, y + y_correction);
                temp_value += Calculate_down_damage(x + x_correction, y + y_correction);
            }
            else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
            {
                temp_value += Calculate_up_gems(x + x_correction, y + y_correction);
                temp_value += Calculate_down_gems(x + x_correction, y + y_correction);
            }
            else
            {
                temp_value += Up_how_much_gems_will_explode(x + x_correction, y + y_correction);
                temp_value += Down_how_much_gems_will_explode(x + x_correction, y + y_correction);
            }
        }
        else if (board_array_master[x, y, 4] == 6)//horiz_and_vertic
        {
            if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
            {
                temp_value += Calculate_horiz_damage_R(x + x_correction, y + y_correction);
                temp_value += Calculate_horiz_damage_L(x + x_correction, y + y_correction);

                temp_value += Calculate_up_damage(x + x_correction, y + y_correction);
                temp_value += Calculate_down_damage(x + x_correction, y + y_correction);
            }
            else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
            {
                temp_value += Calculate_horiz_gems_R(x + x_correction, y + y_correction);
                temp_value += Calculate_horiz_gems_L(x + x_correction, y + y_correction);

                temp_value += Calculate_up_gems(x + x_correction, y + y_correction);
                temp_value += Calculate_down_gems(x + x_correction, y + y_correction);
            }
            else
            {
                temp_value += R_how_much_gems_will_explode(x + x_correction, y + y_correction);
                temp_value += L_how_much_gems_will_explode(x + x_correction, y + y_correction);

                temp_value += Up_how_much_gems_will_explode(x + x_correction, y + y_correction);
                temp_value += Down_how_much_gems_will_explode(x + x_correction, y + y_correction);
            }
        }
        else if (board_array_master[x, y, 4] == 10)//heal hp
        {
            if ((enemy.myCharacter.currentHp  + enemy.myCharacter.heal_me_hp_bonus) <= enemy.myCharacter.maxHp)
                temp_value += enemy.myCharacter.heal_me_hp_bonus;
            else
                temp_value += enemy.myCharacter.maxHp - enemy.myCharacter.currentHp ;

        }
        else if (board_array_master[x, y, 4] == 11)//damage hp
        {
                temp_value += enemy.myCharacter.damage_opponent_bonus;
        }
        return temp_value;
    }


    #region count how much gems will explode
    public int Count_how_much_gems_with_this_color_there_are_around_this_coordinates(int x, int y, int this_color)
    {
        int secondary_explosion_magnitude = 0;

        //vertical
        if ((y - 1 >= 0) && (board_array_master[x, y - 1, 1] == this_color))//check if gem over me have same color of teleported gem
        {
            //check if there is another gem with the same color over
            if ((y - 2 >= 0) && (board_array_master[x, y - 2, 1] == this_color))
            {
                secondary_explosion_magnitude += 2;
                if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 1] == this_color))
                {
                    secondary_explosion_magnitude += 1;
                    //check if there is another gem with the same color under me
                    if (((y + 2) < _Y_tiles) && (board_array_master[x, y + 2, 1] == this_color))
                    {
                        secondary_explosion_magnitude += 1;
                    }
                }
            }
            //check if there is a gem with the same color under me
            else if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 1] == this_color))
            {
                secondary_explosion_magnitude += 2;
                //check if there is another gem with the same color under me
                if (((y + 2) < _Y_tiles) && (board_array_master[x, y + 2, 1] == this_color))
                {
                    secondary_explosion_magnitude += 1;
                }
            }
        }
        else //check if there are 2 gem with the same color under
        {
            if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 1] == this_color)
                && ((y + 2) < _Y_tiles) && (board_array_master[x, y + 2, 1] == this_color)
                 )
            {
                secondary_explosion_magnitude += 2;
            }
        }

        //horizontal
        if ((x - 1 >= 0) && (board_array_master[x - 1, y, 1] == this_color))
        {
            if ((x - 2 >= 0) && (board_array_master[x - 2, y, 1] == this_color))
            {
                secondary_explosion_magnitude += 2;
                if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 1] == this_color))
                {
                    secondary_explosion_magnitude += 1;
                    if (((x + 2) < _X_tiles) && (board_array_master[x + 2, y, 1] == this_color))
                    {
                        secondary_explosion_magnitude += 1;
                    }
                }
            }
            else if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 1] == this_color))
            {
                secondary_explosion_magnitude += 2;
                if (((x + 2) < _X_tiles) && (board_array_master[x + 2, y, 1] == this_color))
                {
                    secondary_explosion_magnitude += 1;
                }
            }
        }
        else
        {
            if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 1] == this_color)
                && ((x + 2) < _X_tiles) && (board_array_master[x + 2, y, 1] == this_color)
                )
            {
                secondary_explosion_magnitude += 2;
            }
        }

        return secondary_explosion_magnitude;
    }




    bool Check_if_this_will_explode(int x, int y)
    {
        if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)) //is a gem
            return true;
        else
            return false;
    }

    int Bomb_how_much_gems_will_explode(int _x, int _y)
    {
        int gems_that_will_explode = 0;

        for (int y = (_y - 1); y < ((_y - 1) + 3); y++)
        {
            if ((y >= 0) && (y < _Y_tiles))
            {
                for (int x = (_x - 1); x < ((_x - 1) + 3); x++)
                {
                    if ((x >= 0) && (x < _X_tiles))
                    {
                        if (board_array_master[x, y, 1] >= 0) //if this tile have something
                        {
                            if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)) //is a gem
                            {
                                if (Check_if_this_will_explode(x, y))
                                    gems_that_will_explode++;
                            }
                        }
                    }
                }
            }
        }
        return gems_that_will_explode;
    }

    int Up_how_much_gems_will_explode(int _x, int _y)
    {
        int gems_that_will_explode = 0;
        for (int y = (_y - 1); y >= 0; y--)
        {
            if ((board_array_master[_x, y, 1] >= 0) && (board_array_master[_x, y, 1] < 30))
            {
                if (Check_if_this_will_explode(_x, y))
                    gems_that_will_explode++;
            }
            else
                return gems_that_will_explode;
        }
        return gems_that_will_explode;
    }

    int Down_how_much_gems_will_explode(int _x, int _y)
    {
        int gems_that_will_explode = 0;
        for (int y = (_y + 1); y < _Y_tiles; y++)
        {
            if ((board_array_master[_x, y, 1] >= 0) && (board_array_master[_x, y, 1] < 30))
            {
                if (Check_if_this_will_explode(_x, y))
                    gems_that_will_explode++;
            }
            else
                return gems_that_will_explode;
        }
        return gems_that_will_explode;
    }

    int R_how_much_gems_will_explode(int _x, int _y)
    {
        int gems_that_will_explode = 0;
        for (int x = (_x + 1); x < _X_tiles; x++)
        {
            if ((board_array_master[x, _y, 1] >= 0) && (board_array_master[x, _y, 1] < 30))
            {
                if (Check_if_this_will_explode(x, _y))
                    gems_that_will_explode++;
            }
            else
                return gems_that_will_explode;
        }
        return gems_that_will_explode;
    }

    int L_how_much_gems_will_explode(int _x, int _y)
    {
        int gems_that_will_explode = 0; ;
        for (int x = (_x - 1); x >= 0; x--)
        {
            if ((board_array_master[x, _y, 1] >= 0) && (board_array_master[x, _y, 1] < 30))
            {
                if (Check_if_this_will_explode(x, _y))
                    gems_that_will_explode++;
            }
            else
                return gems_that_will_explode;
        }
        return gems_that_will_explode;
    }
    #endregion

    #region count useful gems
    bool Check_if_enemy_need_this_gem_color(int g_color)
    {
        if (g_color <= myRuleset.gem_length)
        {
            if ((myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[g_color]
                 - enemy.myCharacter.numberOfGemsCollect[g_color] - temp_enemy_gem_count[g_color]) > 0) //if enemy need this color
            {
                temp_enemy_gem_count[g_color]++;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    int Calculate_bomb_gems(int _x, int _y)
    {
        int useful_gems = 0;

        for (int y = (_y - 1); y < ((_y - 1) + 3); y++)
        {
            if ((y >= 0) && (y < _Y_tiles))
            {
                for (int x = (_x - 1); x < ((_x - 1) + 3); x++)
                {
                    if ((x >= 0) && (x < _X_tiles))
                    {
                        if (board_array_master[x, y, 1] >= 0) //if this tile have something
                        {
                            if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)) //is a gem
                            {
                                if (Check_if_enemy_need_this_gem_color(board_array_master[x, y, 1]))
                                    useful_gems++;
                            }
                        }
                    }
                }
            }
        }
        return useful_gems;
    }

    int Calculate_up_gems(int _x, int _y)
    {
        int useful_gems = 0;
        for (int y = (_y - 1); y >= 0; y--)
        {
            if ((board_array_master[_x, y, 1] >= 0) && (board_array_master[_x, y, 1] < 30))
            {
                if (Check_if_enemy_need_this_gem_color(board_array_master[_x, y, 1]))
                    useful_gems++;
            }
            else
                return useful_gems;
        }
        return useful_gems;
    }

    int Calculate_down_gems(int _x, int _y)
    {
        int useful_gems = 0;
        for (int y = (_y + 1); y < _Y_tiles; y++)
        {
            if ((board_array_master[_x, y, 1] >= 0) && (board_array_master[_x, y, 1] < 30))
            {
                if (Check_if_enemy_need_this_gem_color(board_array_master[_x, y, 1]))
                    useful_gems++;
            }
            else
                return useful_gems;
        }
        return useful_gems;
    }

    int Calculate_horiz_gems_R(int _x, int _y)
    {
        int useful_gems = 0;
        for (int x = (_x + 1); x < _X_tiles; x++)
        {
            if ((board_array_master[x, _y, 1] >= 0) && (board_array_master[x, _y, 1] < 30))
            {
                if (Check_if_enemy_need_this_gem_color(board_array_master[x, _y, 1]))
                    useful_gems++;
            }
            else
                return useful_gems;
        }
        return useful_gems;
    }

    int Calculate_horiz_gems_L(int _x, int _y)
    {
        int useful_gems = 0; ;
        for (int x = (_x - 1); x >= 0; x--)
        {
            if ((board_array_master[x, _y, 1] >= 0) && (board_array_master[x, _y, 1] < 30))
            {
                if (Check_if_enemy_need_this_gem_color(board_array_master[x, _y, 1]))
                    useful_gems++;
            }
            else
                return useful_gems;
        }
        return useful_gems;
    }
    #endregion

    #region calculate damage


    float Calculate_bomb_damage(int _x, int _y)
    {
        float temp_big_damage = 0;
        for (int y = (_y - 1); y < ((_y - 1) + 3); y++)
        {
            if ((y >= 0) && (y < _Y_tiles))
            {
                for (int x = (_x - 1); x < ((_x - 1) + 3); x++)
                {
                    if ((x >= 0) && (x < _X_tiles))
                    {
                        if (board_array_master[x, y, 1] >= 0) //if this tile have something
                        {
                            if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9)) //is a gem
                            {
                                temp_big_damage += Calculate_damage_of_this_gem(board_array_master[x, y, 1]);
                            }
                        }
                    }
                }
            }
        }
        return temp_big_damage;
    }

    float Calculate_up_damage(int _x, int _y)
    {
        float temp_big_damage = 0;
        for (int y = (_y - 1); y >= 0; y--)
        {
            if ((board_array_master[_x, y, 1] >= 0) && (board_array_master[_x, y, 1] < 30))
                temp_big_damage += Calculate_damage_of_this_gem(board_array_master[_x, y, 1]);
            else
                return temp_big_damage;
        }
        return temp_big_damage;
    }

    float Calculate_down_damage(int _x, int _y)
    {
        float temp_big_damage = 0;
        for (int y = (_y + 1); y < _Y_tiles; y++)
        {
            if ((board_array_master[_x, y, 1] >= 0) && (board_array_master[_x, y, 1] < 30))
                temp_big_damage += Calculate_damage_of_this_gem(board_array_master[_x, y, 1]);
            else
                return temp_big_damage;
        }
        return temp_big_damage;
    }

    float Calculate_horiz_damage_R(int _x, int _y)
    {
        float temp_big_damage = 0;
        for (int x = (_x + 1); x < _X_tiles; x++)
        {
            if ((board_array_master[x, _y, 1] >= 0) && (board_array_master[x, _y, 1] < 30))
                temp_big_damage += Calculate_damage_of_this_gem(board_array_master[x, _y, 1]);
            else
                return temp_big_damage;
        }
        return temp_big_damage;
    }

    float Calculate_horiz_damage_L(int _x, int _y)
    {
        float temp_big_damage = 0;
        for (int x = (_x - 1); x >= 0; x--)
        {
            if ((board_array_master[x, _y, 1] >= 0) && (board_array_master[x, _y, 1] < 30))
                temp_big_damage += Calculate_damage_of_this_gem(board_array_master[x, _y, 1]);
            else
                return temp_big_damage;
        }
        return temp_big_damage;
    }

    float[] moveValue;
    void EvalutateMoves()
    {
        print("EvalutateMoves: " + number_of_moves_possible);
        moveValue = new float[number_of_moves_possible];
        if (myRuleset.use_armor)
        {
            for (int i = 0; i < number_of_moves_possible; i++)
            {
                if (board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2], 5] == 0)//this gem don't have moves
                    continue;

                /*
                print(i + "____________________________ " + list_of_moves_possible[i, 1] + "," + list_of_moves_possible[i, 2] + "___ total useful moves of this gem: " + board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2], 5]);
                print("6) " + board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2], 6] + " = down "); //down
                print("7) " + board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2], 7] + " = up "); //up
                print("8) " + board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2], 8] + " = right ");//right
                print("9) " + board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2], 9] + " = left ");//left
                if (board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2], 5] == 0)
                    Debug.LogError(list_of_moves_possible[i, 1] + "," + list_of_moves_possible[i, 2] + " Don't have moves!!!");*/

                    //find moves and double moves
                    if (list_of_moves_possible[i,4] > 0)//if this gem can move down
                {
                    
                    moveValue[i] = Calculate_damage_of_this_gem(list_of_moves_possible[i, 3]) * list_of_moves_possible[i, 4];
                    //print(" = partial value: " + moveValue[i]);

                    //check if the lower gem can also tigger and explosion if move up
                    if (board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2]+1, 7] > 0)//can move up
                    {
                        print(list_of_moves_possible[i, 1] + "," + list_of_moves_possible[i, 2] + " down = double move");
                        moveValue[i] += Calculate_damage_of_this_gem(board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] + 1, 1]) * board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] + 1, 7];
                        //print(" = final value: " + moveValue[i]);
                    }

                    moveValue[i] += EvalutateGainATurn(i, 4);
                    moveValue[i] += EvalutateChargeBonusGemColorNeed(   list_of_moves_possible[i, 4], //main explosion magnitude
                                                                        list_of_moves_possible[i, 3], //main explosion color
                                                                        board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] + 1, 7], //secondary explosion magnitude
                                                                        board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] + 1, 1]); //secondary explosion color
                    //print(" = final value: " + moveValue[i]);
                    continue;
                }
                
                if (list_of_moves_possible[i, 5] > 0)//if this gem can move up
                {
                    
                    moveValue[i] = Calculate_damage_of_this_gem(list_of_moves_possible[i, 3]) * list_of_moves_possible[i, 5];
                    //print("partial value (color: " + list_of_moves_possible[i, 3] + ", quantity: " + list_of_moves_possible[i, 5] + "): " + moveValue[i]);

                    //check if the upper gem can also tigger and explosion if move down
                    if (board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] - 1, 6] > 0)
                    {
                        print(list_of_moves_possible[i, 1] + "," + list_of_moves_possible[i, 2] + " up = double move");
                        //print("partial value (color: " + board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] - 1, 1] + ", quantity: " + board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] - 1, 6] + "): " + moveValue[i]);
                        moveValue[i] += Calculate_damage_of_this_gem(board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] - 1, 1]) * board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] - 1, 6];
                        //print(" = final value: " + moveValue[i]);
                    }

                    moveValue[i] += EvalutateGainATurn(i, 5);
                    moveValue[i] += EvalutateChargeBonusGemColorNeed(   list_of_moves_possible[i, 5], //main explosion magnitude
                                                                        list_of_moves_possible[i, 3], //main explosion color
                                                                        board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] - 1, 6],  //secondary explosion magnitude
                                                                        board_array_master[list_of_moves_possible[i, 1], list_of_moves_possible[i, 2] - 1, 1]); //secondary explosion color
                    print(" = final value: " + moveValue[i]);
                    continue;
                }
                

                
                if (list_of_moves_possible[i, 7] > 0)//if this gem can move left
                {
                    moveValue[i] = Calculate_damage_of_this_gem(list_of_moves_possible[i, 3]) * list_of_moves_possible[i, 7];
                    print(" = partial value: " + moveValue[i]);

                    //check if the left gem can also tigger and explosion if move right
                    if (board_array_master[list_of_moves_possible[i, 1] -1, list_of_moves_possible[i, 2], 8] > 0)
                    {
                        print(list_of_moves_possible[i, 1] + "," + list_of_moves_possible[i, 2] + " left = double move");
                        moveValue[i] += Calculate_damage_of_this_gem(board_array_master[list_of_moves_possible[i, 1] -1, list_of_moves_possible[i, 2], 1]) * board_array_master[list_of_moves_possible[i, 1] -1, list_of_moves_possible[i, 2], 8];
                        //print(" = final value: " + moveValue[i]);
                    }

                    moveValue[i] += EvalutateGainATurn(i, 7);
                    moveValue[i] += EvalutateChargeBonusGemColorNeed(   list_of_moves_possible[i, 7], //main explosion magnitude
                                                                        list_of_moves_possible[i, 3], //main explosion color
                                                                        board_array_master[list_of_moves_possible[i, 1] - 1, list_of_moves_possible[i, 2], 8], //secondary explosion magnitude
                                                                        board_array_master[list_of_moves_possible[i, 1] - 1, list_of_moves_possible[i, 2], 1]); //secondary explosion color
                    print(" = final value: " + moveValue[i]);
                    continue;
                }
                

                if (list_of_moves_possible[i, 6] > 0)//if this gem can move right
                {
                    moveValue[i] = Calculate_damage_of_this_gem(list_of_moves_possible[i, 3]) * list_of_moves_possible[i, 6];
                    //print(" = partial value: " + moveValue[i]);

                    //check if the right gem can also tigger and explosion if move left
                    if (board_array_master[list_of_moves_possible[i, 1] + 1, list_of_moves_possible[i, 2], 9] > 0)
                    {
                        print(list_of_moves_possible[i, 1] + "," + list_of_moves_possible[i, 2] + " right = double move");
                        moveValue[i] += Calculate_damage_of_this_gem(board_array_master[list_of_moves_possible[i, 1] + 1, list_of_moves_possible[i, 2], 1]) * board_array_master[list_of_moves_possible[i, 1] + 1, list_of_moves_possible[i, 2], 9];
                        //print(" = final value: " + moveValue[i]);
                    }
                    
                    moveValue[i] += EvalutateGainATurn(i,6);
                    moveValue[i] += EvalutateChargeBonusGemColorNeed    (list_of_moves_possible[i, 6], //main explosion magnitude
                                                                        list_of_moves_possible[i, 3],  //main explosion color
                                                                        board_array_master[list_of_moves_possible[i, 1] + 1, list_of_moves_possible[i, 2], 9], //secondary explosion magnitude
                                                                        board_array_master[list_of_moves_possible[i, 1] + 1, list_of_moves_possible[i, 2], 1]); //secondary explosion color

                    //print(" = final move value: " + moveValue[i]);
                    continue;
                }
               
            }
        }



        //select best move
        big_move_value = 0;
        int rantomStartPoint = UnityEngine.Random.Range(0, number_of_moves_possible);
        for (int i = rantomStartPoint; i < number_of_moves_possible; i++)
        {
            if (big_move_value < (int)moveValue[i])
            {
                big_move_value = (int)moveValue[i];
                enemy_move_selected = i;
            }
        }
        if (rantomStartPoint != 0)
        {
            for (int i = 0; i < rantomStartPoint; i++)
            {
                if (big_move_value < (int)moveValue[i])
                {
                    big_move_value = (int)moveValue[i];
                    enemy_move_selected = i;
                }
            }
        }

        if (big_move_value <= 0)//if no move matter, pick one random
            enemy_move_selected = UnityEngine.Random.Range(0, number_of_moves_possible);

        print("******** best move value: " + list_of_moves_possible[enemy_move_selected, 1] + "," + list_of_moves_possible[enemy_move_selected, 2] + " = " +big_move_value );
    }

    float EvalutateGainATurn(int moveId, int moveDirection)
    {
        float returnThis = 0;
        float howMuchImportantIsGainAturn = 5;//5 is the arbitrary importace give to the act to gain a move

        if ((!myRuleset.chain_turns_limit) || (enemy.myCharacter.currentChainLenght < myRuleset.max_chain_turns)) //if enemy can gain a turn
        {
            if (myRuleset.gain_turn_if_explode_more_than_3_gems && list_of_moves_possible[moveId, moveDirection] > 3)
                returnThis = howMuchImportantIsGainAturn;

            if (myRuleset.gain_turn_if_explode_same_color_of_previous_move)
            {
                //if this move color is the same of the main or minor explosion
                if (list_of_moves_possible[moveId, 3] == enemy.myCharacter.previous_exploded_color[0] || list_of_moves_possible[moveId, 3] == enemy.myCharacter.previous_exploded_color[1])
                    returnThis = howMuchImportantIsGainAturn;
            }
        }
        return returnThis * howMuchImportantIsGainAturn;
    }

    float EvalutateChargeBonusGemColorNeed(     int mainExplosionMagnitude,
                                                int mainExplosionColor,
                                                int secondaryExplosionMagnitude,
                                                int secondaryExplosionColor)
    {
        float returnThis = 0;

        if (myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge)
        {
            for (int i = 0; i < enemy.myCharacter.bonus_slot_availables; i++)
            {
                if (enemy.myCharacter.bonus_ready[i])//this bonus don't need to be charged
                    continue;

                int bonusColorNeed = enemy.myCharacter.charge_bonus_cost[i] - enemy.myCharacter.filling_bonus[i];

                if (i == mainExplosionColor)
                {
                    if (mainExplosionMagnitude <= bonusColorNeed)
                        returnThis += mainExplosionMagnitude;
                    else
                        returnThis += bonusColorNeed;
                }
                else if (i == secondaryExplosionColor)
                {
                    if (secondaryExplosionMagnitude <= bonusColorNeed)
                        returnThis += secondaryExplosionMagnitude;
                    else
                        returnThis += bonusColorNeed;
                }
            }
        }
        else if (myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
        {
            for (int b = 0; b < enemy.myCharacter.advancedChargeBonuses.Count; b++)//bonus
            {
                if (enemy.myCharacter.bonus_ready[b])//this bonus don't need to be charged
                    continue;

                for (int c = 0; c < enemy.myCharacter.advancedChargeBonuses[b].allowedGemColors.Length ; c++)//color need by this bonus
                {
                    if (!enemy.myCharacter.advancedChargeBonuses[b].allowedGemColors[c]) //ignore this color, bacause this bonus don't need it
                        continue;

 
                    if (enemy.myCharacter.advancedChargeBonuses[b].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
                    {
                        int bonusColorNeed = enemy.myCharacter.advancedChargeBonuses[b].targetTotal  - uIManager.enemyUI.advancedBonusButton[b].currentOrCount;

                        if (c == mainExplosionColor)
                        {
                            if (mainExplosionMagnitude <= bonusColorNeed)
                                returnThis += mainExplosionMagnitude;
                            else
                                returnThis += bonusColorNeed;
                        }
                        else if (c == secondaryExplosionColor)
                        {
                            if (secondaryExplosionMagnitude <= bonusColorNeed)
                                returnThis += secondaryExplosionMagnitude;
                            else
                                returnThis += bonusColorNeed;
                        }
                    }
                    else if (enemy.myCharacter.advancedChargeBonuses[b].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
                    {
                        int bonusColorNeed = 0;
                        for (int n = 0; n < enemy.myCharacter.advancedChargeBonuses[b].targetCostByGemColor.Length; n++)//check the need of each color in the current bonus analyzed
                        {
                            bonusColorNeed = enemy.myCharacter.advancedChargeBonuses[b].targetCostByGemColor[n] - uIManager.enemyUI.advancedBonusButton[b].currentAndCount[n];

                            if (bonusColorNeed == 0)
                                continue;

                            if (c == mainExplosionColor)
                            {
                                if (mainExplosionMagnitude <= bonusColorNeed)
                                    returnThis += mainExplosionMagnitude;
                                else
                                    returnThis += bonusColorNeed;
                            }
                            else if (c == secondaryExplosionColor)
                            {
                                if (secondaryExplosionMagnitude <= bonusColorNeed)
                                    returnThis += secondaryExplosionMagnitude;
                                else
                                    returnThis += bonusColorNeed;
                            }
                        }
                    }
                }
            }
        }

        return returnThis * enemy.myCharacter.howMuchImportantIs_ChargeBonuses;
    }

    float Calculate_damage_of_this_gem(int this_gem_color)
    {
        if (this_gem_color >= 7) //this is not a gem
            return 0;

        float returnThis = 0;

        returnThis += Calculate_DamageOpponent(this_gem_color); //this consider also armor, so "howMuchImportantIs_" is add inside the Calculate_DamageOpponent() function
        returnThis += Calculate_DamageMe(this_gem_color) * enemy.myCharacter.howMuchImportantIs_AvoidDamage;
        returnThis += Calculate_HealtPotential(this_gem_color) * enemy.myCharacter.howMuchImportantIs_HealMe;

        //print("____________Calculate_damage_of_this_gem[" + this_gem_color + "] = " + returnThis);

        return returnThis;
    }

    int Calculate_HealtPotential(int this_gem_color)
    {

        int returnThis = 0;

        if (myRuleset.gemExplosionOutcomes[this_gem_color].healMe <= 0)
            return 0;

        if (myRuleset.gemExplosionOutcomes[this_gem_color].healMe <= (enemy.myCharacter.maxHp - enemy.myCharacter.currentHp ))
            returnThis = myRuleset.gemExplosionOutcomes[this_gem_color].healMe;
        else
            returnThis = enemy.myCharacter.maxHp - enemy.myCharacter.currentHp ;


        return returnThis;
    }


    float Calculate_DamageMe(int this_gem_color)
    {
        if (myRuleset.gemExplosionOutcomes[this_gem_color].damageMe <= 0)
            return 0;

        float temp_big_damage = 0;
        if (this_gem_color < myRuleset.gem_length)
        {

            switch (enemy.myCharacter.armor[this_gem_color])
            {
                case Character.gemColorArmor.weak:
                    temp_big_damage += myRuleset.gemExplosionOutcomes[this_gem_color].damageMe * 2 * enemy.myCharacter.howMuchImportantIs_AvoidDamage;
                    break;


                case Character.gemColorArmor.normal:
                    temp_big_damage += myRuleset.gemExplosionOutcomes[this_gem_color].damageMe * enemy.myCharacter.howMuchImportantIs_AvoidDamage;
                    break;


                case Character.gemColorArmor.strong:
                    temp_big_damage += myRuleset.gemExplosionOutcomes[this_gem_color].damageMe * 0.5f * enemy.myCharacter.howMuchImportantIs_AvoidDamage;
                    break;


                case Character.gemColorArmor.immune:

                    break;

                case Character.gemColorArmor.absorb: //absorb the damage of a damageMe gem = get healt
                    if ((enemy.myCharacter.currentHp  + myRuleset.gemExplosionOutcomes[this_gem_color].damageMe) > enemy.myCharacter.maxHp)
                        temp_big_damage -= (enemy.myCharacter.maxHp - enemy.myCharacter.currentHp ) * enemy.myCharacter.howMuchImportantIs_HealMe;
                    else
                        temp_big_damage -= myRuleset.gemExplosionOutcomes[this_gem_color].damageMe * enemy.myCharacter.howMuchImportantIs_HealMe;
                    break;

                case Character.gemColorArmor.repel: //repel the damage of a damageMe gem = damage player
                        temp_big_damage -= myRuleset.gemExplosionOutcomes[this_gem_color].damageMe * enemy.myCharacter.howMuchImportantIs_DealDamage;
                    break;
            }
        }

        if ((enemy.myCharacter.currentHp  - temp_big_damage) <= 0)
            temp_big_damage += 99999;

        return temp_big_damage * -1;//*-1 to invert the value of this gem explosion, because this damage is against me and I don't want it
    }

    float Calculate_DamageOpponent(int this_gem_color)
    {
        if (myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent <= 0)
            return 0;

        float temp_big_damage = 0;
        if (this_gem_color < myRuleset.gem_length)
        {

            switch (player.myCharacter.armor[this_gem_color])
            {
                case Character.gemColorArmor.weak:
                    temp_big_damage += myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent * 2 * enemy.myCharacter.howMuchImportantIs_DealDamage;
                    break;


                case Character.gemColorArmor.normal:
                    temp_big_damage += myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent * enemy.myCharacter.howMuchImportantIs_DealDamage;
                    break;


                case Character.gemColorArmor.strong:
                    temp_big_damage += myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent * 0.5f * enemy.myCharacter.howMuchImportantIs_DealDamage;
                    break;


                case Character.gemColorArmor.immune:

                    break;

                case Character.gemColorArmor.absorb:
                    if ((player.myCharacter.currentHp + myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent) > player.myCharacter.maxHp)
                        temp_big_damage -= (player.myCharacter.maxHp - player.myCharacter.currentHp) * enemy.myCharacter.howMuchImportantIs_NotHealThePlayer;
                    else
                        temp_big_damage -= myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent * enemy.myCharacter.howMuchImportantIs_NotHealThePlayer;
                    break;

                case Character.gemColorArmor.repel:
                    if ((enemy.myCharacter.currentHp  - myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent) < 0)
                        temp_big_damage -= 99999;
                    else
                        temp_big_damage -= myRuleset.gemExplosionOutcomes[this_gem_color].damageOpponent * enemy.myCharacter.howMuchImportantIs_AvoidDamage;
                    break;
            }
        }

        return temp_big_damage;
    }

    #endregion

    void Search_big_move()
    {
        print("Search_big_move");
        big_move_value = 0; //reset varaible before use it
        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
            Search_big_damage();
        else
            Search_big_explosion();
    }

    void Search_big_damage()
    {
        Debug.Log("Search_big_damage");
        int temp_big_damage = 0;
        int big_damage = 0;

        if (myRuleset.use_armor)
        {
            for (int i = 0; i < number_of_gems_moveable; i++)
            {
                
                if ((player.myCharacter.armor[list_of_moves_possible[i, 3]] != Character.gemColorArmor.absorb)
                    && (player.myCharacter.armor[list_of_moves_possible[i, 3]] != Character.gemColorArmor.immune)
                    && (player.myCharacter.armor[list_of_moves_possible[i, 3]] != Character.gemColorArmor.repel))
                {
                    if (player.myCharacter.armor[list_of_moves_possible[i, 3]] == Character.gemColorArmor.weak)
                        temp_big_damage = list_of_moves_possible[i, 0] * 2;
                    else if (player.myCharacter.armor[list_of_moves_possible[i, 3]] == Character.gemColorArmor.normal)
                        temp_big_damage = list_of_moves_possible[i, 0];
                    else if (player.myCharacter.armor[list_of_moves_possible[i, 3]] == Character.gemColorArmor.strong)
                        temp_big_damage = (int)(list_of_moves_possible[i, 0] * 0.5f);


                    if (temp_big_damage > big_damage)
                    {
                        big_damage = temp_big_damage;
                        enemy_move_selected = i;
                    }
                    else if (temp_big_damage == big_damage)
                    {
                        if (UnityEngine.Random.Range(1, 100) > UnityEngine.Random.Range(40, 60)) //random update gem with the same explosive power
                            enemy_move_selected = i;
                    }
                }
            }
        }

        big_move_value = big_damage;
    }

    void Search_big_explosion()//search only explosion >= 4
    {
        //Debug.Log("Search_big_explosion");
        if (myRuleset.use_armor)
        {
            Debug.Log("armor mode");
            for (int i = 0; i < number_of_gems_moveable; i++)
            {
                if ((player.myCharacter.armor[list_of_moves_possible[i, 3]] != Character.gemColorArmor.absorb)
                || (player.myCharacter.armor[list_of_moves_possible[i, 3]] != Character.gemColorArmor.repel)) //gems color to avoid
                {

                    if (list_of_moves_possible[i, 0] >= 4)//if this move can explode 4 or more gems
                    {
                        if (enemy_move_selected == -1)
                            enemy_move_selected = i;
                        else
                        {
                            if (list_of_moves_possible[i, 0] > list_of_moves_possible[enemy_move_selected, 0]) //if there are a better gem, select it
                                enemy_move_selected = i;
                            else if (list_of_moves_possible[i, 0] == list_of_moves_possible[enemy_move_selected, 0])
                            {
                                if (UnityEngine.Random.Range(1, 100) > UnityEngine.Random.Range(40, 60)) //random update gem with the same explosive power
                                    enemy_move_selected = i;
                            }
                        }
                    }

                }
            }
        }
        else
        {
            //Debug.Log("NOT armor mode");
            for (int i = 0; i < number_of_gems_moveable; i++)
            {

                if ((myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[list_of_moves_possible[i, 3]]
                 - enemy.myCharacter.numberOfGemsCollect[list_of_moves_possible[i, 3]]) > 0) //if enemy need this color
                {
                    if (list_of_moves_possible[i, 0] >= 4)//if this move can explode 4 or more gems
                    {
                        if (enemy_move_selected == -1)
                            enemy_move_selected = i;
                        else
                        {
                            if (list_of_moves_possible[i, 0] > list_of_moves_possible[enemy_move_selected, 0]) //if there are a better gem, select it
                                enemy_move_selected = i;
                            else if (list_of_moves_possible[i, 0] == list_of_moves_possible[enemy_move_selected, 0])
                            {
                                if (UnityEngine.Random.Range(1, 100) > UnityEngine.Random.Range(40, 60)) //random update gem with the same explosive power
                                    enemy_move_selected = i;
                            }
                        }
                    }
                }

            }
        }

        if (enemy_move_selected >= 0)
            big_move_value = list_of_moves_possible[enemy_move_selected, 0];

        //Debug:
        /*
		if (enemy_move_selected >= 0)
			{
			Debug.Log ("Search_big_explosion: " + list_of_moves_possible[enemy_move_selected,1] 
			           + ";" + list_of_moves_possible[enemy_move_selected,2] 
			           + " exp = " + list_of_moves_possible[enemy_move_selected,0]);
			}
		else
			Debug.Log("no big explosion found");
			*/
    }

    int Search_most_big_explosion_with_this_color(int color_explosion)
    {
        Debug.Log("Search_most_big_explosion_with_this_color: " + color_explosion);
        int gem_with_most_big_explosion = 0;

        if (gem_color[color_explosion].Count > 0)
        {
            gem_with_most_big_explosion = ((int)gem_color[color_explosion][0]);
            for (int i = 0; i < gem_color[color_explosion].Count; i++)
            {
                /*
				Debug.Log("check :" //+ list_of_moves_possible[(int)gem_color[color_explosion][i],1] + "," + + list_of_moves_possible[(int)gem_color[color_explosion][i],2] 
				          + "; color: " + list_of_moves_possible[(int)gem_color[color_explosion][i],3]
				         // + "; exp: " + list_of_moves_possible[(int)gem_color[color_explosion][i],0]
				          );
				Debug.Log ("if " + list_of_moves_possible[(int)gem_color[color_explosion][i],0] + " > " + list_of_moves_possible[gem_with_most_big_explosion,0]);
				*/
                if (list_of_moves_possible[(int)gem_color[color_explosion][i], 0] > list_of_moves_possible[gem_with_most_big_explosion, 0])
                {
                    gem_with_most_big_explosion = ((int)gem_color[color_explosion][i]);
                    //Debug.Log("***color: " + list_of_moves_possible[gem_with_most_big_explosion,3]);
                }
                else if (list_of_moves_possible[(int)gem_color[color_explosion][i], 0] == list_of_moves_possible[gem_with_most_big_explosion, 0])
                {
                    if (UnityEngine.Random.Range(1, 100) > UnityEngine.Random.Range(40, 60)) //random update gem with the same explosive power
                        gem_with_most_big_explosion = ((int)gem_color[color_explosion][i]);
                }
            }

            /*
			Debug.Log("Search_most_big_explosion_with_this_color, find move: " 
			         // + list_of_moves_possible[gem_with_most_big_explosion,1] + "," + + list_of_moves_possible[gem_with_most_big_explosion,2]
			          + "; color: " + list_of_moves_possible[gem_with_most_big_explosion,3]
			         // + "; exp: " + list_of_moves_possible[gem_with_most_big_explosion,0]
			          );*/
            return gem_with_most_big_explosion;
        }
        else
            return -1;//no explosion with this color
    }

    void Search_same_color_of_previous_move()
    {
        Debug.Log("Search_same_color_of_previous_move");
        if ((enemy.myCharacter.previous_exploded_color[0] < 0) && (enemy.myCharacter.previous_exploded_color[1] < 0))
            return;

        int best_explosion_with_previous_main_gem_color = -1;
        int best_explosion_with_previous_minor_gem_color = -1;

        //Debug.Log("previous main: " + enemy_previous_exploded_color[0]);
        //Debug.Log("previous minor: " + enemy_previous_exploded_color[1]);

        //check previous main color
        if ((enemy.myCharacter.previous_exploded_color[0] >= 0) //il previous main gem have exploded something
            && (gem_color[enemy.myCharacter.previous_exploded_color[0]].Count > 0)) //and there are same color available now
        {
            if (myRuleset.use_armor)
            {
                if ((player.myCharacter.armor[enemy.myCharacter.previous_exploded_color[0]] != Character.gemColorArmor.absorb) || (player.myCharacter.armor[enemy.myCharacter.previous_exploded_color[0]] != Character.gemColorArmor.repel)) //avoid to use a color useful for the player
                    best_explosion_with_previous_main_gem_color = Search_most_big_explosion_with_this_color(enemy.myCharacter.previous_exploded_color[0]);
            }
            else
                best_explosion_with_previous_main_gem_color = Search_most_big_explosion_with_this_color(enemy.myCharacter.previous_exploded_color[0]);

        }

        //check previous minor color
        if ((enemy.myCharacter.previous_exploded_color[1] >= 0) //il previous minor gem have exploded something
            && (gem_color[enemy.myCharacter.previous_exploded_color[1]].Count > 0)) //and there are same color available now
        {
            if (myRuleset.use_armor)
            {
                if ((player.myCharacter.armor[enemy.myCharacter.previous_exploded_color[1]] != Character.gemColorArmor.absorb) || (player.myCharacter.armor[enemy.myCharacter.previous_exploded_color[1]] != Character.gemColorArmor.repel)) //avoid to use a color useful for the player
                    best_explosion_with_previous_minor_gem_color = Search_most_big_explosion_with_this_color(enemy.myCharacter.previous_exploded_color[1]);
            }
            else
                best_explosion_with_previous_minor_gem_color = Search_most_big_explosion_with_this_color(enemy.myCharacter.previous_exploded_color[1]);
        }

        //choose the best color between main and minor
        if ((best_explosion_with_previous_main_gem_color >= 0) && (best_explosion_with_previous_minor_gem_color >= 0))
        {
            if (list_of_moves_possible[best_explosion_with_previous_main_gem_color, 0] >= list_of_moves_possible[best_explosion_with_previous_minor_gem_color, 0])
                enemy_move_selected = list_of_moves_possible[best_explosion_with_previous_main_gem_color, 0];
            else
                enemy_move_selected = list_of_moves_possible[best_explosion_with_previous_minor_gem_color, 0];
        }
        else if (best_explosion_with_previous_main_gem_color >= 0)
            enemy_move_selected = best_explosion_with_previous_main_gem_color;
        else if (best_explosion_with_previous_minor_gem_color >= 0)
            enemy_move_selected = best_explosion_with_previous_minor_gem_color;

        //debug
        /*
		if (enemy_move_selected >= 0)
			{
			Debug.Log("enemy_move_selected " + enemy_move_selected);
			Debug.Log("Search_same_color_of_previous_move find: " + list_of_moves_possible[enemy_move_selected,1] + ";" + list_of_moves_possible[enemy_move_selected,2] + "exp = " + list_of_moves_possible[enemy_move_selected,0]);
			}*/
    }


    void Arrange_gems_by_effectiveness_against_armor()
    {
        int temp_count = 1000;
        int temp_array_place = 0;
        bool[] gem_aready_checked = new bool[myRuleset.gem_length];
        for (int n = 0; n < myRuleset.gem_length; n++)
        {
            for (int i = 0; i < myRuleset.gem_length; i++)
            {
                if (gem_aready_checked[i] == false)
                {
                    if ((int)player.myCharacter.armor[i] <= temp_count)
                    {
                        temp_count = (int)player.myCharacter.armor[i];
                        temp_array_place = i;
                        enemy_AI_preference_order[n] = (enemy_AI_manual_setup)i;
                    }
                }
                if (i == myRuleset.gem_length - 1)//if the loop is end
                {
                    //check the gem
                    gem_aready_checked[temp_array_place] = true;
                    temp_count = 1000;
                }
            }
        }
    }


    void Arrange_gems_by_necessity()
    {
        //Arrange gem priority from gems more need to less need
        int temp_count = 0;
        int temp_array_place = 0;
        bool[] gem_aready_checked = new bool[myRuleset.gem_length];
        for (int n = 0; n < myRuleset.gem_length; n++)
        {
            for (int i = 0; i < myRuleset.gem_length; i++)
            {
                if (gem_aready_checked[i] == false)
                {
                    //Debug.Log("i = " + i + "number_of_gems_to_destroy_to_win[i] = "  + number_of_gems_to_destroy_to_win[i] + " ** enemy.numberOfGemsCollect[i] = " + enemy.numberOfGemsCollect[i]);
                    if ((myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[i] - enemy.myCharacter.numberOfGemsCollect[i]) > temp_count)
                    {
                        temp_count = (myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[i] - enemy.myCharacter.numberOfGemsCollect[i]);
                        temp_array_place = i;
                        enemy_AI_preference_order[n] = (enemy_AI_manual_setup)i;
                    }
                }
                if (i == myRuleset.gem_length - 1)//if loop end
                {
                    //check the gem
                    gem_aready_checked[temp_array_place] = true;
                    temp_count = 0;
                }
            }
        }
    }

    void Enemy_search_main_gem()
    {
        for (int i = 0; i < enemy_AI_preference_order.Length; i++)
        {
            foreach (enemy_AI_manual_setup p in Enum.GetValues(typeof(enemy_AI_manual_setup)))
            {
                if ((enemy_AI_preference_order[i] == p))//&& ((int)p < myRuleset.gem_length) )
                {
                    if (gem_color[(int)p].Count > 0)
                    {
                        if ((gem_color[(int)p].Count > 1) && ((myRuleset.enemies[currentEnemy].number_of_gems_to_destroy_to_win[(int)p] - enemy.myCharacter.numberOfGemsCollect[(int)p]) > 0))
                            enemy_move_selected = ((int)gem_color[(int)p][UnityEngine.Random.Range(0, (int)gem_color[(int)p].Count)]);
                        else
                            enemy_move_selected = ((int)gem_color[(int)p][0]);

                        return;
                    }
                }

            }
            if (i == enemy_AI_preference_order.Length - 1)// If you can't find a useful move, choose one by random
            {
                Debug.Log("If you can't find a useful move, choose one by random");
                enemy_move_selected = UnityEngine.Random.Range(0, number_of_gems_moveable - 1);
            }
        }
    }

    void Enemy_select_minor_gem()
    {
        //Debug.LogWarning("Enemy_select_minor_gem() " + enemy_move_selected);
        available_directions = new ArrayList();

        //debug
        /*
        int totalCount = 0;
        for (int m = 0; m < list_of_moves_possible.GetLength(0); m++)
        {
            int directionCount = 0;
            for (int i = 4; i <= 7; i++)//search big explosion
            {
                if (list_of_moves_possible[m, i] <= 0)
                    continue;

                directionCount++;
                totalCount++;
            }

            if (directionCount == 0)
                Debug.LogError("["+m+"] " +list_of_moves_possible[m, 1] + ","+ list_of_moves_possible[m, 2] + " explosion:  " + list_of_moves_possible[m, 0] + "   BUT  list_of_moves_possible   NO DIRECTION!!!!");
        }
        Debug.LogWarning("totalCount " + totalCount);
        */

        if (search_best_move)
        {
            int best_direction = 4;

            for (int i = 4; i <= 7; i++)//search big explosion
            {
                //if (list_of_moves_possible[enemy_move_selected, i] <= 0) //don't solve the bug because the list MUST DON'T have  <= 0
                //continue;

                //Debug.LogWarning("list_of_moves_possible[enemy_move_selected, i] " + list_of_moves_possible[enemy_move_selected, i]);

                if (list_of_moves_possible[enemy_move_selected, i] > list_of_moves_possible[enemy_move_selected, best_direction])
                {
                    //Debug.LogWarning("A");
                    best_direction = i;
                }
                else if (list_of_moves_possible[enemy_move_selected, i] == list_of_moves_possible[enemy_move_selected, best_direction])
                {
                    if (UnityEngine.Random.Range(1, 100) > UnityEngine.Random.Range(40, 60)) //random update gem with the same explosive power
                    {
                       // Debug.LogWarning("B");//BUG HERE
                        best_direction = i;
                    }
                }
            }
           // Debug.LogWarning(" = best_direction " + best_direction);
            enemy_move_direction = best_direction;
        }
        else
        {
            for (int i = 4; i <= 7; i++)
            {
                if (list_of_moves_possible[enemy_move_selected, i] > 0)
                {
                   // Debug.LogWarning("Add " + i);
                    available_directions.Add(i);
                }
            }

            if (available_directions.Count > 1)
            {
               // Debug.LogWarning(" = (int)available_directions[UnityEngine.Random.Range(0, available_directions.Count)];");
                enemy_move_direction = (int)available_directions[UnityEngine.Random.Range(0, available_directions.Count)];
            }
            else
                {
               // Debug.LogWarning(" = (int)available_directions[0]");
                enemy_move_direction = (int)available_directions[0];
                }
            }

    }


    void Subdivide_moves_by_color()
    {
        gem_color = new ArrayList[myRuleset.gem_length];
        for (int i = 0; i < myRuleset.gem_length; i++)
            gem_color[i] = new ArrayList();

        for (int i = 0; i < number_of_gems_moveable; i++)
        {
            gem_color[list_of_moves_possible[i, 3]].Add(i);
        }
      
    }


    void Enemy_select_main_gem(int move_number)
    {
        cursor.position = script_tiles_array[list_of_moves_possible[move_number, 1], list_of_moves_possible[move_number, 2]].transform.position;
        script_tiles_array[list_of_moves_possible[move_number, 1], list_of_moves_possible[move_number, 2]].I_become_main_gem();

    }

    void Enemy_move()
    {

        if (enemy_move_direction == 4)//down
        {
            tile_C script_minor_gem = script_tiles_array[list_of_moves_possible[enemy_move_selected, 1], list_of_moves_possible[enemy_move_selected, 2] + 1];
            script_minor_gem.I_become_minor_gem(enemy_move_direction);
        }
        else if (enemy_move_direction == 5)//up
        {
            tile_C script_minor_gem = script_tiles_array[list_of_moves_possible[enemy_move_selected, 1], list_of_moves_possible[enemy_move_selected, 2] - 1];
            script_minor_gem.I_become_minor_gem(enemy_move_direction);

        }
        else if (enemy_move_direction == 6)//R
        {
            tile_C script_minor_gem = script_tiles_array[list_of_moves_possible[enemy_move_selected, 1] + 1, list_of_moves_possible[enemy_move_selected, 2]];
            script_minor_gem.I_become_minor_gem(enemy_move_direction);

        }
        else if (enemy_move_direction == 7)//L
        {

            //Debug.LogWarning(list_of_moves_possible[enemy_move_selected, 1] + "," + list_of_moves_possible[enemy_move_selected, 2]); //BUG with 0,0

            tile_C script_minor_gem = script_tiles_array[list_of_moves_possible[enemy_move_selected, 1] - 1, list_of_moves_possible[enemy_move_selected, 2]];
            script_minor_gem.I_become_minor_gem(enemy_move_direction);
        }
    }


    public void ExecuteThisMove_Debug(int moveId)
    {

        Enemy_select_main_gem(moveId);
        Enemy_select_minor_gem();
        Enemy_move();

    }
}
