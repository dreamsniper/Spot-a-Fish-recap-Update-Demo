using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public partial class UIManager : MonoBehaviour {


    public void Reset_charge_fill()
    {
        if (board.myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge)
        {
            if (board.player_turn)
                playerUI.gui_bonus_ico_array[slot_bonus_ico_selected].GetComponent<ChargeBonusButton>().Reset_fill();
            else
                enemyUI.gui_bonus_ico_array[board.enemy_chosen_bonus_slot].GetComponent<ChargeBonusButton>().Reset_fill();
        }
        else if (board.myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
        {
            if (board.player_turn)
                playerUI.advancedBonusButton[slot_bonus_ico_selected].Reset_fill();
            else
                enemyUI.advancedBonusButton[slot_bonus_ico_selected].Reset_fill();
        }
    }

    public void Update_left_moves_text()//call from Auto_setup_gui(), Switch_gem(int direction), Move_gems_to_target_positions(), Detect_which_gems_will_explode(int __x, int __y, int _gem), Update_turn_order_after_a_bad_move()
    {
        board.player.myUI.gui_left_moves.text = "Left moves: " + board.player.myCharacter.currentMovesLeft;
        board.enemy.myUI.gui_left_moves.text = "Left moves: " + board.enemy.myCharacter.currentMovesLeft;
    }

    public void Update_token_count()//call from Create_new_board(), tile_C.Check_the_content_of_this_tile()
    {
        if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.take_all_tokens)
        {
            playerUI.gui_token_count_text.text = board.number_of_token_collected.ToString() + " / " + board.number_of_token_to_collect.ToString();
            if (board.number_of_token_collected >= board.number_of_token_to_collect)
            {
                playerUI.gui_token_count_text.color = color_collect_done;
                board.Player_win();
            }
        }
    }

    public void This_gem_color_is_collected(int this_color)//call from tile_C.Update_gems_score()
    {
        if (!board.activeCharacter.myCharacter.thisGemColorIsCollected[this_color])
        {
            board.activeCharacter.myCharacter.thisGemColorIsCollected[this_color] = true;
            board.activeCharacter.myUI.gui_count.transform.GetChild(this_color).transform.GetChild(1).GetComponent<Text>().color = color_collect_done;
        }

        board.activeCharacter.myUI.gui_progressBar_slider[2].value = board.activeCharacter.myCharacter.totalNumberOfGemsRequiredColletted + board.activeCharacter.myCharacter.totalNumberOfExtraGemsCollettedAfterTheRequired;
    }

    public void Update_board_hp()//call from tile_C.Update_tile_hp()
    {
        board.HP_board--;
        gui_board_hp_slider.value++;

        if (board.HP_board < 1 && board.myRuleset.emit_token_only_after_all_tiles_are_destroyed)
            board.allow_token_emission = true;

    }

    public void Update_bonus_fill(int _x, int _y, int n)//call from tile_C.Update_gems_score()
    {
        //if this bonus don't is full yet
        if (board.activeCharacter.myUI.gui_bonus_ico_array[n] && (board.activeCharacter.myCharacter.filling_bonus[n] < board.activeCharacter.myCharacter.charge_bonus_cost[n]))
        {
            board.activeCharacter.myCharacter.filling_bonus[n]++;
            board.activeCharacter.myUI.gui_bonus_ico_array[n].GetComponent<bonus_button>().Update_fill();
        }

    }

    public void Update_inventory_bonus(int bonus_id, int quantity)//call from tile_C: Give_more_time(), Destroy_one(), Destroy_horizontal(), Destroy_all_gems_with_this_color(), Update_tile_hp(), Destroy_3x3(), Destroy_vertical(), Destroy_horizonal_and_vertical(), Check_the_content_of_this_tile(), Check_if_shuffle_is_done()
    {
        //print("Update_inventory_bonus " + board.player_turn);
        if (board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory)
        {
            board.activeCharacter.myCharacter.bonus_inventory[bonus_id] += quantity;
            board.activeCharacter.myUI.bonus_inventory_script.Update_bonus_count(bonus_id);
            if (quantity < 1)
                board.activeCharacter.myUI.bonus_inventory_script.Deselect(bonus_id);
        }
    }

    public void Update_score()//call from initiate_ugui(), Order_to_gems_to_explode(), tile_C.Update_tile_hp()
    {

        board.activeCharacter.myUI.gui_score.text = "score: " + board.activeCharacter.myCharacter.score;

        if ((board.activeCharacter.isPlayer && board.myRuleset.win_requirement_selected == Ruleset.win_requirement.reach_target_score)
            || (!board.activeCharacter.isPlayer && board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score))
            {
            board.activeCharacter.myUI.gui_progressBar_slider[2].value = board.activeCharacter.myCharacter.score;
            board.activeCharacter.myUI.progressBarText.text = board.activeCharacter.myCharacter.score + " / " + board.activeCharacter.myCharacter.target_score[2];
            }


        if (board.myRuleset.whenShowWinScreen != Ruleset.WhenShowWinScreen.ReachMinimumWinRequisite && board.player_win)
        {
            if (board.myRuleset.win_requirement_selected != Ruleset.win_requirement.collect_gems)
            {
                if ((board.current_star_score < 3) && (board.player.myCharacter.score >= board.player.myCharacter.target_score[2]))
                {
                    board.current_star_score = 3;

                    if (board.myRuleset.whenShowWinScreen == Ruleset.WhenShowWinScreen.ContinuePlayUntilLose)
                        board.globalRules.praise_script.Win_and_continue_to_play(board.current_star_score - 1);
                    else if (board.myRuleset.whenShowWinScreen == Ruleset.WhenShowWinScreen.Reach3StarsWinRequisite)
                        {
                        print("end game and call win screen");
                        board.player_can_move = false;
                        board.game_end = true;
                        }
                }
                else if ((board.current_star_score < 2) && (board.player.myCharacter.score >= board.player.myCharacter.target_score[1]))
                {
                    board.current_star_score = 2;
                    board.globalRules.praise_script.Win_and_continue_to_play(board.current_star_score - 1);
                }
            }
        }

        if (board.menuKitBridge && board.menuKitBridge.Stage_uGUI_obj)//use menu kit win screen
        {
            board.menuKitBridge.UpdateScore(board.activeCharacter.myCharacter.score);
        }
    }

    void UpdateCharaterHP(Board_C.BoardCharacter thisCharacter)
    {

        if (thisCharacter.myCharacter.currentHp <= thisCharacter.myCharacter.previousHp) //damage
        {
            thisCharacter.myUI.gui_hp_slider.value = thisCharacter.myCharacter.currentHp;
            thisCharacter.myUI.gui_secondarySliderImage.color = color_hp_damage;
            thisCharacter.myUI.gui_hp_secondarySlider.value = thisCharacter.myCharacter.previousHp;

            if (thisCharacter.myCharacter.currentHp == thisCharacter.myCharacter.previousHp)
            {
                if (board.audioManager.enemySFX.unharmed && board.player_turn)
                    board.audioManager.enemySFX.HPAudioSource.PlayOneShot(board.audioManager.enemySFX.unharmed);
            }
            else
            {
                if (thisCharacter.isPlayer)
                {
                    if (board.audioManager.playerSFX.loseHP)
                    {
                        if (!board.audioManager.playerSFX.HPAudioSource.isPlaying)
                            board.audioManager.playerSFX.HPAudioSource.PlayOneShot(board.audioManager.playerSFX.loseHP);
                    }
                }
                else
                {
                    if (board.audioManager.enemySFX.loseHP)
                    {
                        if (!board.audioManager.enemySFX.HPAudioSource.isPlaying)
                            board.audioManager.enemySFX.HPAudioSource.PlayOneShot(board.audioManager.enemySFX.loseHP);
                    }
                }
            }
        }
        else //heal
        {
            thisCharacter.myUI.gui_secondarySliderImage.color = color_hp_heal;
            thisCharacter.myUI.gui_hp_secondarySlider.value = thisCharacter.myCharacter.currentHp;


            if (thisCharacter.isPlayer)
            {
                if (board.audioManager.playerSFX.gainHP)
                {
                    if (!board.audioManager.playerSFX.HPAudioSource.isPlaying)
                        board.audioManager.playerSFX.HPAudioSource.PlayOneShot(board.audioManager.playerSFX.gainHP);
                }
            }
            else
            {
                if (board.audioManager.enemySFX.gainHP)
                {
                    if (!board.audioManager.enemySFX.HPAudioSource.isPlaying)
                        board.audioManager.enemySFX.HPAudioSource.PlayOneShot(board.audioManager.enemySFX.gainHP);
                }
            }

        }

        thisCharacter.myUI.gui_hp_text.text = thisCharacter.myCharacter.currentHp.ToString() + " / " + thisCharacter.myCharacter.maxHp.ToString();
        
    }

    public void Update_hp()//call from Board_C.Heal_me()
    {

        if (!board.myRuleset.use_armor)
            return;


        //enemy
        UpdateCharaterHP(board.enemy);

        if (board.enemy.myCharacter.currentHp == 0 && board.myRuleset.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
        {
            board.Player_win(true);
            return;
        }


        //player
        UpdateCharaterHP(board.player);
        

        if (board.player.myCharacter.currentHp == 0 && board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
            board.Player_lose();

    }

    public void Slider_hp_animation(Board_C.BoardCharacter thisCharacter)
    {

        if (thisCharacter.isPlayer && board.myRuleset.lose_requirement_selected != Ruleset.lose_requirement.player_hp_is_zero)
            return;

        if (!thisCharacter.isPlayer && board.myRuleset.win_requirement_selected != Ruleset.win_requirement.enemy_hp_is_zero)
            return;

        if (thisCharacter.myCharacter.currentHp == thisCharacter.myCharacter.previousHp)
            return;

        if (thisCharacter.myUI.gui_hp_secondarySlider.value == thisCharacter.myUI.gui_hp_slider.value)
        {
            thisCharacter.myCharacter.previousHp = thisCharacter.myCharacter.currentHp;
            thisCharacter.myUI.hpSliderAnimationTime = 0.0f;
            return;
        }

        thisCharacter.myUI.hpSliderAnimationTime += 2.5f * Time.deltaTime;


        if (thisCharacter.myCharacter.currentHp < thisCharacter.myCharacter.previousHp)
            thisCharacter.myUI.gui_hp_secondarySlider.value = Mathf.Lerp(thisCharacter.myCharacter.previousHp, thisCharacter.myUI.gui_hp_slider.value, thisCharacter.myUI.hpSliderAnimationTime);
        else //heal animation
            thisCharacter.myUI.gui_hp_slider.value = Mathf.Lerp(thisCharacter.myUI.gui_hp_slider.value, thisCharacter.myCharacter.currentHp, thisCharacter.myUI.hpSliderAnimationTime);
    }

    public void UpdateAllAdvancedChargeBonusUGUI()
    {

        for (int i = 0; i < board.activeCharacter.myUI.advancedBonusButton.Length; i++)
        {
            if (board.activeCharacter.myUI.advancedBonusButton[i].GetMyRule() != AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
                continue;

            board.activeCharacter.myUI.advancedBonusButton[i].currentOrCount = 0;

            for (int c = 0; c < board.activeCharacter.myCharacter.advancedChargeBonuses[i].allowedGemColors.Length; c++)
            {
                if (!board.activeCharacter.myCharacter.advancedChargeBonuses[i].allowedGemColors[c])
                    continue;


                board.activeCharacter.myUI.advancedBonusButton[i].currentOrCount += board.activeCharacter.myCharacter.gemColorAdvancedChargeBonusPool[c];
            }
        }

        for (int i = 0; i < board.activeCharacter.myUI.advancedBonusButton.Length; i++)
            board.activeCharacter.myUI.advancedBonusButton[i].UpdateGUI();
        

    }
}
