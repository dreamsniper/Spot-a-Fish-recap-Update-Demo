using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour {

    bool game_is_ended;

    void Game_end()
    {
        if (game_is_ended)
            return;

        game_is_ended = true;

        cursor.gameObject.SetActive(false);

        if (player_win)
            Show_win_screen();
		else
            Show_lose_screen();
    }

    public void Player_win(bool forceTurnUpdate = false)
    {
        if (!game_end && !player_win)
        {

            player_win = true;

            if (myRuleset.whenShowWinScreen == Ruleset.WhenShowWinScreen.ReachMinimumWinRequisite)
            {
                game_end = true;
                player_can_move = false;
            }
            else
            {
                globalRules.praise_script.Win_and_continue_to_play(0);
                if (current_star_score < 1)
                    current_star_score = 1;
            }
            //Debug.LogWarning("You win!");

            if (forceTurnUpdate)
                UpdateTurn();
        }
    }

    public void Player_lose()
    {
        if (!game_end)
        {
            game_end = true;
            if (player_can_move)
            {
                player_can_move = false;
                UpdateTurn();
            }
            //Debug.LogWarning("You lose!");
        }
    }

    void Show_win_screen()
    {
        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
        {
            player.myCharacter.score += (globalRules.every_hp_saved_give * player.myCharacter.currentHp);

            //calculate star score
            if (myRuleset.threeStarScore_onLoseRequirement)
            {
                current_star_score = 0;
                float percentualHPLeft = player.myCharacter.currentHp / player.myCharacter.maxHp * 100;
                for (int i = 0; i < myRuleset.percentualPlayerHPSparedForStarScore.Length; i++)
                {
                    if (percentualHPLeft > myRuleset.percentualTimeSparedForThreeStarScore[i])
                        current_star_score++;
                }
            }
        }
        else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.timer)
        {
            player.myCharacter.score += (int)(globalRules.every_second_saved_give * time_left);

            //calculate star score
            if (myRuleset.threeStarScore_onLoseRequirement)
            {
                current_star_score = 0;
                float percentualTimeLeft = time_left / myRuleset.timer * 100;
                for (int i = 0; i < myRuleset.percentualTimeSparedForThreeStarScore.Length; i++)
                {
                    if (percentualTimeLeft > myRuleset.percentualTimeSparedForThreeStarScore[i])
                        current_star_score++;
                }
            }
        }
        else if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
        {
            player.myCharacter.score += (globalRules.every_move_saved_give * player.myCharacter.currentMovesLeft);

            //calculate star score
            if (myRuleset.threeStarScore_onLoseRequirement)
            {
                current_star_score = 0;
                for (int i = 0; i <   myRuleset.movesToSparedForThreeStarScore.Length; i++)
                {
                    if (player.myCharacter.currentMovesLeft > myRuleset.movesToSparedForThreeStarScore[i])
                        current_star_score++;
                }
            }
        }

        if (myRuleset.threeStarScore_onWinRequirement)
        {
            current_star_score = 0;
            if (myRuleset.win_requirement_selected == Ruleset.win_requirement.reach_target_score)
            {
                for (int i = 0; i < player.myCharacter.target_score.Length; i++)
                {
                    if (player.myCharacter.score > player.myCharacter.target_score[i])
                        current_star_score++;
                }
            }
            else if (myRuleset.win_requirement_selected == Ruleset.win_requirement.collect_gems)
            {
                for (int i = 0; i < myRuleset.player.additionalGemsToCollecForStarScore.Length; i++)
                {
                    print(i + " ... " +player.myCharacter.totalNumberOfExtraGemsCollettedAfterTheRequired + " > " + myRuleset.player.additionalGemsToCollecForStarScore[i]);
                    if (player.myCharacter.totalNumberOfExtraGemsCollettedAfterTheRequired > myRuleset.player.additionalGemsToCollecForStarScore[i])
                        current_star_score++;
                }
            }
        }


        if (menuKitBridge.Stage_uGUI_obj)//use menu kit win screen
        {
            menuKitBridge.WinScreen_menuKit(current_star_score);
        }
        else //use default win screen
        {
            uIManager.ShowWinScreen();
            //Debug.LogWarning("show win screen!");
            print("Stars: " + current_star_score);
            audioManager.Play_sfx(audioManager.win_sfx);
        }
    }


    void Show_lose_screen()
    {
        if (menuKitBridge.Stage_uGUI_obj)//use menu kit win screen
        {
            menuKitBridge.LoseScreen_menuKit();
        }
        else //use default win screen
        {
            uIManager.gui_lose_screen.SetActive(true);
            //Debug.LogWarning("show lose screen!");
            audioManager.Play_sfx(audioManager.lose_sfx);
        }
    }



}
