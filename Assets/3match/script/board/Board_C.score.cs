using UnityEngine;
using System.Collections;

using System;

public partial class Board_C : MonoBehaviour
{

    [HideInInspector] public int current_star_score;
    [HideInInspector] public int score_of_this_turn_move;
    //[HideInInspector] public int n_gems_exploded_with_main_gem;
    //[HideInInspector] public int n_gems_exploded_with_minor_gem;

    int explode_same_color_again_with = 0;//0 = false; 1 = with main gem; 2 =with minor gem



    void Calculate_primary_explosion_score(int number_of_elements_to_damage_temp)
    {

        //print("... globalRules.score_reward_for_explode_gems " + globalRules.score_reward_for_explode_gems.Length);

 

        if (explode_same_color_again_with == 0) //no same color
        {
            //Debug.Log(n_gems_exploded_with_main_gem + " and " + n_gems_exploded_with_minor_gem);
            if ((mainGemLatestSwitchInfo.n_gems_exploded >= 3) && (minorGemLatestSwitchInfo.n_gems_exploded >= 3))
                {
                //print("n_gems_exploded_with_main_gem: " + n_gems_exploded_with_main_gem);
                //print("n_gems_exploded_with_minor_gem: " + n_gems_exploded_with_minor_gem);
                score_of_this_turn_move = globalRules.score_reward_for_explode_gems[mainGemLatestSwitchInfo.n_gems_exploded - 3] + globalRules.score_reward_for_explode_gems[minorGemLatestSwitchInfo.n_gems_exploded - 3]; // "-3" because the array length (because 0, 1 and 2 explosion are impossible)
                }
            else if (mainGemLatestSwitchInfo.n_gems_exploded >= 3)
            {
                //print("n_gems_exploded_with_main_gem: " + n_gems_exploded_with_main_gem);
                score_of_this_turn_move = globalRules.score_reward_for_explode_gems[mainGemLatestSwitchInfo.n_gems_exploded - 3];
            }
            else if (minorGemLatestSwitchInfo.n_gems_exploded >= 3)
            {
                //print("n_gems_exploded_with_minor_gem: " + n_gems_exploded_with_minor_gem);
                score_of_this_turn_move = globalRules.score_reward_for_explode_gems[minorGemLatestSwitchInfo.n_gems_exploded - 3];
            }
        }
        else if (explode_same_color_again_with == 1 && mainGemLatestSwitchInfo.n_gems_exploded >= 3) //same color with main gem
        {
            //print("n_gems_exploded_with_main_gem: " + n_gems_exploded_with_main_gem);
            score_of_this_turn_move = globalRules.score_reward_for_explode_gems[mainGemLatestSwitchInfo.n_gems_exploded - 3];

            score_of_this_turn_move += (int)Math.Ceiling(activeCharacter.myCharacter.explode_same_color_n_turn * globalRules.score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);

            if (minorGemLatestSwitchInfo.n_gems_exploded >= 3)
                score_of_this_turn_move += globalRules.score_reward_for_explode_gems[minorGemLatestSwitchInfo.n_gems_exploded - 3];
        }
        else if (explode_same_color_again_with == 2 && minorGemLatestSwitchInfo.n_gems_exploded >= 3) //same color with minor gem
        {
            score_of_this_turn_move = globalRules.score_reward_for_explode_gems[minorGemLatestSwitchInfo.n_gems_exploded - 3];

            score_of_this_turn_move += (int)Math.Ceiling(activeCharacter.myCharacter.explode_same_color_n_turn * globalRules.score_reward_for_explode_gems_of_the_same_color_in_two_or_more_turns_subsequently);

            if (mainGemLatestSwitchInfo.n_gems_exploded > 0)
                score_of_this_turn_move += globalRules.score_reward_for_explode_gems[mainGemLatestSwitchInfo.n_gems_exploded - 3];
        }

        if (player_turn)
        {
            player.myCharacter.score += score_of_this_turn_move;
            if (globalRules.praise_the_player)
            {
                if (globalRules.for_big_explosion)
                {
                    if (gems_useful_moved == 1 && number_of_elements_to_damage_temp > 3)
                    {
                        globalRules.praise_obj.SetActive(true);
                        globalRules.praise_script.Big_explosion(number_of_elements_to_damage_temp);
                    }
                    else if ((gems_useful_moved == 2)
                             && (mainGemLatestSwitchInfo.n_gems_exploded > 3 || minorGemLatestSwitchInfo.n_gems_exploded > 3))
                    {
                        if (mainGemLatestSwitchInfo.n_gems_exploded >= minorGemLatestSwitchInfo.n_gems_exploded)
                        {
                            globalRules.praise_obj.SetActive(true);
                            globalRules.praise_script.Big_explosion(mainGemLatestSwitchInfo.n_gems_exploded);
                        }
                        else
                        {
                            globalRules.praise_obj.SetActive(true);
                            globalRules.praise_script.Big_explosion(minorGemLatestSwitchInfo.n_gems_exploded);
                        }
                    }
                }

                if (myRuleset.gain_turn_if_explode_same_color_of_previous_move || number_of_elements_to_damage_temp <= 3)
                {
                    if (globalRules.for_explode_same_color_again && player.myCharacter.explode_same_color_n_turn > 0)
                    {
                        globalRules.praise_obj.SetActive(true);
                        globalRules.praise_script.Combo_color(player.myCharacter.explode_same_color_n_turn);
                    }
                }

            }
        }
        else
            enemy.myCharacter.score += score_of_this_turn_move;
    }

    void Calculate_secondary_explosion_score(int number_of_elements_to_damage_temp)
    {
        score_of_this_turn_move = 0;
        score_of_this_turn_move = (int)Math.Ceiling(globalRules.score_reward_for_each_explode_gems_in_secondary_explosion * number_of_elements_to_damage * (1 + (n_combo * globalRules.score_reward_for_secondary_combo_explosions)));

        activeCharacter.myCharacter.score += score_of_this_turn_move;

        if (myRuleset.gain_turn_if_secondary_explosion && number_of_elements_to_damage >= myRuleset.seconday_explosion_maginiture_needed_to_gain_a_turn && n_combo >= myRuleset.combo_lenght_needed_to_gain_a_turn && bonus_select == Bonus.None)
        {
            Gain_turns(1);
        }
    }

    void Calculate_score()//(int number_of_elements_to_damage_temp, string calledFrom)
    {

        bool update_score = false;
        if (number_of_elements_to_damage_with_SwitchingGems > 0)
        {
            update_score = true;
            Calculate_primary_explosion_score(number_of_elements_to_damage_with_SwitchingGems);

            //if (player_can_move_when_gem_falling)
                //number_of_elements_to_damage_with_SwitchingGems = 0;

        }
        if (number_of_elements_to_damage > 0)
        {
            update_score = true;
            Calculate_secondary_explosion_score(number_of_elements_to_damage+number_of_elements_to_damage_with_bonus);

            if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
                number_of_elements_to_damage = 0;
        }
        if (update_score)
        {
            uIManager.Update_score();
            Check_win_score_condition();
        }
    }

    void Check_win_score_condition()
    {
        if (myRuleset.win_requirement_selected == Ruleset.win_requirement.reach_target_score)
        {
            if (myRuleset.threeStarScore_onWinRequirement)
            {
                if (player_turn && (player.myCharacter.score >= player.myCharacter.target_score[0]))
                    Player_win();
            }
            else
                {
                if (player_turn && (player.myCharacter.score >= player.myCharacter.target_score[2]))
                    Player_win();
                }
    }

        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score)
        {
            if (!player_turn && (enemy.myCharacter.score >= enemy.myCharacter.target_score[2]))
                Player_lose();
        }
    }
}
