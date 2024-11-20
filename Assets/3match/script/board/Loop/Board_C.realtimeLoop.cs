using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour
{


    void StartBoardUpdate()
    {
        print("StartBoardUpdate");
        player_turn = true;
        player_can_move = true;
        start_time = Time.timeSinceLevelLoad;
        stage_started = true;
        current_moveStatus = moveStatus.waitingNewMove;
    }

    void BoardUpdate()
    {
        if (!shuffle_ongoing)
            {
            if (game_end)
                {
                Game_end();
                return;
                }

            ResetAlreadyCheckedMarks();

            //player input
            if (current_moveStatus == moveStatus.switchingGems)
            {
                SwitchingGems();
            }


            //read
            //Check_vertical_falling_bottom_to_top();

            //Debug.Log(" Check");
            Check_new_gem_to_generate();

            Check_vertical_falling();
            Check_diagonal_falling();

            Check_secondary_explosions();

            UpdateRealtimeIceLoop();

            Check_ALL_possible_moves();
            //Debug.Log("END Check");

            


            if (BoardUpdating())
            {
                //Debug.Log("Cancell_hint");
                Cancell_hint();

                //create
                //Debug.Log("Generate_new_gems");
                Generate_new_gems();
                //execute
                //Debug.Log("ExecuteChanges");
                ExecuteChanges();
                //calculate score
                //Debug.Log("Calculate_score");
                Calculate_score();
                number_of_elements_to_damage_with_SwitchingGems = 0;
                //Debug.Log("Calculate_End");

                //reset tile check under immovable elements
                foreach (tile_C tile in immovable_elements)
                {
                    for (int yy = tile._y + 1; yy < _Y_tiles; yy++)
                    {
                        if ((board_array_master[tile._x, yy, 0] > -1) && (board_array_master[tile._x, yy, 1] == -99))
                        {
                            board_array_master[tile._x, yy, 13] = 0;

                        }
                        else
                            break;
                    }
                }
            }
            else
            {

                CheckIfThereAreMissingFallingElemets();

                //Debug.Log("Check_if_show_hint");
                Check_if_show_hint();
                //Debug.Log("Check_if_show_hint END");
            }


            
        }


    }

 
    public void CheckIfThereAreMissingFallingElemets()
    {

        if (number_of_gems_to_move < 1)
            return;

        int fallingCount = 0;

        for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (script_tiles_array[x, y] == null)
                    continue;

                if (script_tiles_array[x, y].isFalling)
                    fallingCount++;


            }
        }


        if (number_of_gems_to_move > fallingCount)
        {
            for (int y = 0; y < _Y_tiles - 1; y++)//don't check last line
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if (script_tiles_array[x, y] == null)
                        continue;

                    
                    if (board_array_master[x, y, 13] == 1)
                        continue;

                    if (board_array_master[x, y, 11] != 0)
                        continue;

                    if (script_tiles_array[x,y].myContent == null && board_array_master[x, y, 1] == -99)
                    {
                        Debug.LogWarning(x + "," + y + " must require the content of the tile above");

                        board_array_master[x, y, 11] = 0;
                        board_array_master[x, y, 13] = 0;

                        if (y-1 >= 0)
                        {
                            board_array_master[x, y-1, 11] = 0;
                            board_array_master[x, y-1, 13] = 0;
                        }
                    }
                    

                }
            }
        }

    }



    void ResetAlreadyCheckedMarks()
    {
        for (int y = _Y_tiles - 1; y >= 0; y--)//from bottom to top
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (board_array_master[x, y, 11] == 0)
                    board_array_master[x, y, 13] = 0;
            }
        }
    }

    bool BoardUpdating()
    {
        bool return_this = false;

        //print(number_of_elements_to_damage + " ... " + number_of_elements_to_damage_with_SwitchingGems + " ... " + number_of_moves_possible + " ... " + number_of_gems_to_move + " ... " + number_of_new_gems_to_create);
        int elements_to_update = (primary_explosion_count + number_of_elements_to_damage + number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage_with_bonus + number_of_gems_to_move + number_of_new_gems_to_create);

        if (forceExecuteChangesUntilExplosionIsSee)
            return true;

        if (shuffle_ongoing || elements_to_update > 0)
            return_this = true;

        primary_explosion_count = 0;

        return return_this;
    }


    bool forceExecuteChangesUntilExplosionIsSee;
    public void ExecuteChanges()//public only for debug button UI
    {
        for (int y = _Y_tiles - 1; y >= 0; y--)//from bottom to top
        {
            for (int x = 0; x < _X_tiles; x++)
            {

                if (board_array_master[x, y, 1] < 0 || script_tiles_array[x, y].myContent == null) //if no tile or no gem
                    continue; //skip this x,y


                if (board_array_master[x, y, 11] == 1 || board_array_master[x, y, 11] == 666)//destroy
                {
                   board_array_master[x, y, 11] = 111; // explosion ongoing

                    script_tiles_array[x, y].Explosion();

                    forceExecuteChangesUntilExplosionIsSee = false;

                }
                else if (board_array_master[x, y, 11] >= 3 && board_array_master[x, y, 11] <= 5)//falling
                {
                    script_tiles_array[x, y].Fall_by_one_step(board_array_master[x, y, 11] - 3);
                }

            }

        }
    }



    void Generate_new_gems()//call from BoardUpdate()
    {
        if (myRuleset.gem_emitter_rule == Ruleset.gem_emitter.off)
        {
            number_of_new_gems_to_create = 0;
            return;
        }

        for (int i = 0; i < number_of_tiles_leader; i++)
        {
            if (board_array_master[tiles_leader_array[i]._x, tiles_leader_array[i]._y, 11] == 2)//creation
                script_tiles_array[tiles_leader_array[i]._x, tiles_leader_array[i]._y].CreationStart();
            
        }

        //do the same under the generative blocks
        for (int i = 0; i < generativeBlockInfos.Count; i++)
        {
            if (!generativeBlockInfos[i].generatorIsOn)
                continue;


            print("generativeBlockInfos[i].myGenerativeTargertPosition.y " + generativeBlockInfos[i].myGenerativeTargertPosition.y);


            if (board_array_master[generativeBlockInfos[i].myGenerativeTargertPosition.x, generativeBlockInfos[i].myGenerativeTargertPosition.y, 11] == 2)//creation
                script_tiles_array[generativeBlockInfos[i].myGenerativeTargertPosition.x, generativeBlockInfos[i].myGenerativeTargertPosition.y].CreationStart();

        }
    }


}
