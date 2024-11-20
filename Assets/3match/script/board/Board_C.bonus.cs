using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Board_C : MonoBehaviour
{


    [HideInInspector]public Bonus bonus_select;
    [HideInInspector] public int number_of_bonus_on_board;

    bool clickable_bonus_on_boad;
    bool free_switchable_bonus_on_boad;


    //these variables help to stop the board analysis when AI find the most high value possible
    int max_value_destroy_one;
    int max_value_switch_gem_teleport_click1;
    int max_value_switch_gem_teleport_click2;
    int max_value_destroy_3x3;
    int max_value_destroy_horizontal;
    int max_value_destroy_vertical;
    int max_value_destroy_horizontal_and_vertical;

    public void Heal_me(int _heal)//call from bonus_button.Activate() or tile_C.bonus
    {
        print("Heal_me: " + _heal);
        if ((activeCharacter.myCharacter.currentHp + _heal) <= activeCharacter.myCharacter.maxHp)
            activeCharacter.myCharacter.currentHp += _heal;
        else
            activeCharacter.myCharacter.currentHp = activeCharacter.myCharacter.maxHp;

        uIManager.Update_hp();
    }

    public void Damage_opponent(int _damage)//call from bonus_button.Activate() or tile_C.bonus
    {
        print("--Damage_opponent:" + _damage);
        if (passiveCharacter == null)
        {
            Debug.LogWarning("There are no enemy in this stage!");
            return;
        }

        if ((passiveCharacter.myCharacter.currentHp - _damage) <= 0)
            passiveCharacter.myCharacter.currentHp = 0;
        else
            passiveCharacter.myCharacter.currentHp -= _damage;

        uIManager.Update_hp();
    }

    void Destroy_all_gems_with_this_color(int this_color)
    {
        if (this_color >= 0 && this_color <= 6)
        {

            Debug.Log("Destroy_all_gems_with_this_color (board version)");
            player_can_move = false;
            cursor.gameObject.SetActive(false);

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (board_array_master[x, y, 1] == this_color) //if this gem have my same color
                {
                    Annotate_explosions(x, y, Board_C.ExplosionCause.bonus);
                }
            }
        }

        uIManager.Reset_charge_fill();
        uIManager.Update_inventory_bonus(7, -1);

        audioManager.Play_bonus_sfx(7);
        Order_to_gems_to_explode();
        Update_on_board_bonus_count();
        }
    }
    

    public void Update_on_board_bonus_count()
    {
        if (number_of_bonus_on_board > 0)
            number_of_bonus_on_board--;

        //Debug.Log("Update_on_board_bonus_count: " + number_of_bonus_on_board);
    }

    public void Search_max_bonus_values_for_charge_bonus()//call from Create_new_board()
    {
        if (myRuleset.versus && (
            (myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge) //calculate the max value that be archive using the bonus on board
            || (myRuleset.trigger_by_select == Ruleset.trigger_by.inventory)
            || myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
            )
        {
            //search most long x_line and y_line
            int most_long_x_line = 0;
            int temp_x_line = 0;
            int most_long_y_line = 0;
            int temp_y_line = 0;

            //search x_line
            for (int y = 0; y < _Y_tiles; y++)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if (board_array_master[x, y, 0] >= 0) //if there is a tile here
                    {
                        temp_x_line++;
                    }
                    else //line discontinued, so check its length
                    {
                        //Debug.Log("x line break at " + x + "," + y);
                        if (temp_x_line > most_long_x_line)
                            most_long_x_line = temp_x_line;
                    }
                }
                //reach the end of this x line
                if (temp_x_line > most_long_x_line)
                    most_long_x_line = temp_x_line;

                temp_x_line = 0;

                if (most_long_x_line == _X_tiles)
                {
                    //Debug.Log("you can't find a X line more long of " + most_long_x_line + " so stop the check");
                    break;
                }

            }
            //Debug.Log("the most long x_line is: " + most_long_x_line);

            //search y_line
            for (int x = 0; x < _X_tiles; x++)
            {
                for (int y = 0; y < _Y_tiles; y++)
                {
                    if (board_array_master[x, y, 0] >= 0) //if there is a tile here
                    {
                        temp_y_line++;
                    }
                    else //line discontinued, so check its length
                    {
                        //Debug.Log("y line break at " + x + "," + y);
                        if (temp_y_line > most_long_y_line)
                            most_long_y_line = temp_y_line;
                    }
                }
                //reach the end of this y line
                if (temp_y_line > most_long_y_line)
                    most_long_y_line = temp_y_line;

                temp_y_line = 0;

                if (most_long_y_line == _Y_tiles)
                {
                    //Debug.Log("you can't find a Y line more long of " + most_long_y_line + " so stop the check");
                    break;
                }
            }
            //Debug.Log("the most long y_line is: " + most_long_y_line);


            switch (myRuleset.lose_requirement_selected)
            {
                case Ruleset.lose_requirement.enemy_collect_gems:
                    max_value_destroy_one = 7;
                    max_value_switch_gem_teleport_click1 = 9;
                    max_value_switch_gem_teleport_click2 = 9;
                    max_value_destroy_3x3 = 9;
                    max_value_destroy_horizontal = most_long_x_line;
                    max_value_destroy_vertical = most_long_y_line;
                    max_value_destroy_horizontal_and_vertical = most_long_x_line + most_long_y_line - 1;
                    break;

                case Ruleset.lose_requirement.player_hp_is_zero:
                    max_value_destroy_one = 7 * gem_damage_opponent_max_value * 2;
                    max_value_switch_gem_teleport_click1 = (9 * gem_damage_opponent_max_value * 2);
                    max_value_switch_gem_teleport_click2 = (9 * gem_damage_opponent_max_value);
                    max_value_destroy_3x3 = (6 * gem_damage_opponent_max_value * 2) + (3 * gem_damage_opponent_max_value);
                    max_value_destroy_horizontal = (int)Math.Round((most_long_x_line * 0.65f * gem_damage_opponent_max_value * 2) + (most_long_x_line * 0.35f * gem_damage_opponent_max_value));
                    max_value_destroy_vertical = (int)Math.Round((most_long_y_line * 0.65f * gem_damage_opponent_max_value * 2) + (most_long_y_line * 0.35f * gem_damage_opponent_max_value));
                    max_value_destroy_horizontal_and_vertical = (int)Math.Round((most_long_x_line * 0.65f * gem_damage_opponent_max_value * 2) + (most_long_x_line * 0.35f * gem_damage_opponent_max_value) + (most_long_y_line * 0.65f * gem_damage_opponent_max_value * 2) + (most_long_y_line * 0.35f * gem_damage_opponent_max_value) - gem_damage_opponent_max_value);
                    break;

                case Ruleset.lose_requirement.enemy_reach_target_score:
                    max_value_destroy_one = 9;
                    max_value_switch_gem_teleport_click1 = 9;
                    max_value_switch_gem_teleport_click2 = 9;
                    max_value_destroy_3x3 = 9;
                    max_value_destroy_horizontal = most_long_x_line;
                    max_value_destroy_vertical = most_long_y_line;
                    max_value_destroy_horizontal_and_vertical = (most_long_x_line + most_long_y_line - 1);
                    break;
            }



        }
    }

}
