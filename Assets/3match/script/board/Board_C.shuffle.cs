using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour
{
    //bool can_shuffle_now;
    [HideInInspector]public bool shuffle_ongoing;
    int shuffleAttemptsCount;

    void Check_if_shuffle()
    {
        if (myRuleset.no_more_moves_rule_selected == Ruleset.no_more_moves_rule.shuffle)
        {
            if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
            {
                if (!BoardUpdating())
                    Shuffle();
            }
            else
                Shuffle();
        }
        else //lose when no moves
            LoseBecauseNoMoreMoves(false);
    }

    void LoseBecauseNoMoreMoves(bool noMoreShuffles)
    {
        if(!noMoreShuffles)
        {
            if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime && BoardUpdating())
            return;
        }

        if (myRuleset.win_requirement_selected == Ruleset.win_requirement.destroy_all_gems && total_gems_on_board_at_start <= 0)
            player_win = true;
        else
            player_win = false;

        player_can_move = false;
        game_end = true;
        Game_end();
    }

    //shuffle gems when no move available
    void Shuffle()//call from Check_ALL_possible_moves()
    {
        if (free_switchable_bonus_on_boad)
            return;

        if (!shuffle_ongoing)
        {

            if (!ShuffleSafetuCheck())//not safe
            return;

            shuffle_ongoing = true;

            shuffleAttemptsCount++;
            if (shuffleAttemptsCount > myRuleset.maxShuffleAttempts)
                {
                LoseBecauseNoMoreMoves(true);
                return;
                }


            //Debug.Log("shuffle");
            for (int y = 0; y < _Y_tiles; y++)
            {
                for (int x = 0; x < _X_tiles; x++)
                {

                    if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9) //there is a gem
                        && (board_array_master[x, y, 3] == 0))//and without padlock
                    {
                        number_of_gems_to_mix++;
                        board_array_master[x, y, 1] = UnityEngine.Random.Range(0, myRuleset.gem_length);
                        board_array_master[x, y, 4] = 0; //reset bonus
                        Avoid_triple_color_gem(x, y);
                        //update gem
                        script_tiles_array[x,y].StartCoroutine(script_tiles_array[x, y].Shuffle_update());
                    }
                }
            }
        }

    }

    bool ShuffleSafetuCheck()
    {

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {

                if (board_array_master[x, y, 11] != 0)//the board is updating, so don't need shuffle
                {
                    //print("shuffle aborted");
                    return false;
                }
            }
        }

        return true;
    }


    public void Avoid_triple_color_gem(int x, int y)//call from Shuffle(), Create_new_board()
    {
        int attempt_count = 0;
        if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] < 9))//if this is a gem
        {
            if (((x + 1 < _X_tiles) && (x - 1 >= 0)) && ((y + 1 < _Y_tiles) && (y - 1 >= 0)))
            {
                attempt_count = 0;
                while ((attempt_count <= myRuleset.gem_length)
                       && ((board_array_master[x, y, 1] == board_array_master[x + 1, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y, 1])
                    || (board_array_master[x, y, 1] == board_array_master[x, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 1, 1])))
                {
                    //vertical check
                    if (board_array_master[x, y, 1] + 1 < myRuleset.gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;

                    attempt_count++;
                }
            }
            else if (((x + 1 < _X_tiles) && (x - 1 >= 0)))
            {
                attempt_count = 0;
                while ((attempt_count <= myRuleset.gem_length)
                       && ((board_array_master[x, y, 1] == board_array_master[x + 1, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y, 1])))
                {
                    //horizontal check
                    if (board_array_master[x, y, 1] + 1 < myRuleset.gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;

                    attempt_count++;
                }
            }
            else if (((y + 1 < _Y_tiles) && (y - 1 >= 0)))
            {
                attempt_count = 0;
                while ((attempt_count <= myRuleset.gem_length)
                       && ((board_array_master[x, y, 1] == board_array_master[x, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 1, 1])))
                {
                    //Debug.Log("while verticale");
                    if (board_array_master[x, y, 1] + 1 < myRuleset.gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;

                    attempt_count++;
                }
            }
            if ((x - 2 >= 0) && (x - 1 >= 0))
                if ((board_array_master[x, y, 1] == board_array_master[x - 1, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 2, y, 1]))
                {
                    if (board_array_master[x, y, 1] + 1 < myRuleset.gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;
                }
            if ((y - 2 >= 0) && (y - 1 >= 0))
                if ((board_array_master[x, y, 1] == board_array_master[x, y - 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 2, 1]))
                {
                    if (board_array_master[x, y, 1] + 1 < myRuleset.gem_length)
                        board_array_master[x, y, 1]++;
                    else
                        board_array_master[x, y, 1] = 0;
                }
        }
    }

}
