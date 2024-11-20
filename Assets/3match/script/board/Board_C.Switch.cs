using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Board_C : MonoBehaviour
{

    public enum moveStatus//avoid multiple moves in the same step
    {
        waitingNewMove,
        switchingGems,
        switchingGemsAnimationOngoing
    }
    [HideInInspector] public moveStatus current_moveStatus;

    [HideInInspector] public int temp_direction = -1;


    public bool[,] position_of_gems_that_will_explode;
    /* [0 main_gem ; 1 minor_gem]
         *0 = the one 2 to up
         *1 = the one 1 to up
         *2 = the one 2 to right
         *3 = the one 1 to right
         *4 = the one 2 to down
         *5 = the one 1 to down
         *6 = the one 2 to left
         *7 = the one 1 to left
    */
    [HideInInspector] public int number_of_padlocks_involved_in_explosion;
    [HideInInspector] public int number_of_elements_to_damage;
    [HideInInspector] public int number_of_elements_to_damage_with_bonus;
    [HideInInspector] public int number_of_elements_to_damage_with_SwitchingGems;
    [HideInInspector] public int gems_useful_moved;//help to know if the move is a double-move (if = 2: main gem AND minor gem will explode)

    public enum ExplosionCause
    {
        switchingGems,
        bonus,
        secondayExplosion
    }


    [HideInInspector] public TileContent avatar_main_gem;
    [HideInInspector] public int main_gem_selected_x = -10;
    [HideInInspector] public int main_gem_selected_y = -10;
    [HideInInspector] public int main_gem_color = -10;

    [HideInInspector] public TileContent avatar_minor_gem;
    [HideInInspector] public int minor_gem_destination_to_x = -10;
    [HideInInspector] public int minor_gem_destination_to_y = -10;
    [HideInInspector] public int minor_gem_color = -10;

    public struct LatestSwitchInfo
    {
        public bool is2x2Explosion;
        public MyDirections switchDiretion;
        public int n_gems_exploded;
    }
    LatestSwitchInfo mainGemLatestSwitchInfo;
    LatestSwitchInfo minorGemLatestSwitchInfo;

    void AnnotateSwitchDirection()
    {

        if (main_gem_selected_x > minor_gem_destination_to_x)
        {
            mainGemLatestSwitchInfo.switchDiretion = MyDirections.Left;
            minorGemLatestSwitchInfo.switchDiretion = MyDirections.Right;
        }

        if (main_gem_selected_x < minor_gem_destination_to_x)
        {
            mainGemLatestSwitchInfo.switchDiretion = MyDirections.Right;
            minorGemLatestSwitchInfo.switchDiretion = MyDirections.Left;
        }

        if (main_gem_selected_y > minor_gem_destination_to_y)
        {
            mainGemLatestSwitchInfo.switchDiretion = MyDirections.Up;
            minorGemLatestSwitchInfo.switchDiretion = MyDirections.Down;
        }

        if (main_gem_selected_y < minor_gem_destination_to_y)
        {
            mainGemLatestSwitchInfo.switchDiretion = MyDirections.Down;
            minorGemLatestSwitchInfo.switchDiretion = MyDirections.Up;
        }

    }

    public enum MyDirections
    { 
        None,
        Up,
        Down,
        Right,
        Left
    }
    public MyDirections switchDiretion;

    public void SwitchingGems()
    {

        if (current_moveStatus != moveStatus.switchingGems)
            return;

        print("SwitchingGems()");

        current_moveStatus = moveStatus.switchingGemsAnimationOngoing;

        //Switching gems ongoing
        board_array_master[main_gem_selected_x, main_gem_selected_y, 11] = 6;
        board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] = 6;


        //reset variables

        reserved_for_primary_explosion.Clear();
        number_of_elements_to_damage_with_SwitchingGems = 0;
        explode_same_color_again_with = 0;//0 = false; 1 = with main gem; 2 =with minor gem
        score_of_this_turn_move = 0;
        mainGemLatestSwitchInfo = new LatestSwitchInfo();
        minorGemLatestSwitchInfo = new LatestSwitchInfo();
        //n_gems_exploded_with_main_gem = 0;
        //n_gems_exploded_with_minor_gem = 0;

        
        if (myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
            {
            cursor.gameObject.SetActive(false);
            player_can_move = false;
            }

        switchDiretion = MyDirections.None;

        //if this move make explode something
        if (This_switch_will_produce_an_explosion())
        {
            AnnotateSwitchDirection();

            Move_gems_to_target_positions();
            cameraController.Center_camera_to_move(main_gem_selected_x, main_gem_selected_y, _X_tiles, _Y_tiles,  /*pivot_board,*/  globalRules.accuracy);

            //create array to note the explosions
            position_of_gems_that_will_explode = new bool[2, 8];

            if (Main_gem_explode())
            {

                //Debug.LogWarning("--------------Main_gem_explode()");
                gems_useful_moved++;
                //n_gems_exploded_with_main_gem++;
                mainGemLatestSwitchInfo.n_gems_exploded++;


                Check_if_this_gem_have_the_same_color_of_the_gem_exploded_in_the_previous_move(board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1]);

                List<Vector2Int> squareExplosions = new List<Vector2Int>();
                if (myRuleset.allow2x2Explosions)
                    squareExplosions = GetSquare2x2ExplosionInfoList(main_gem_selected_x, main_gem_selected_y, switchDiretion); //thisGemCan2x2Explode[new Vector2Int(main_gem_selected_x, main_gem_selected_y)].GetExplosionByDirection(switchDiretion);

                if (squareExplosions.Count > 0)
                {
                    //Debug.Log("main 2 x 2  count: " + squareExplosions.Count);
                    mainGemLatestSwitchInfo.is2x2Explosion = true;
                }

                Detect_which_gems_will_explode(minor_gem_destination_to_x, minor_gem_destination_to_y, 0, squareExplosions);
            }
            else
            {
                //no color to keep track here
                activeCharacter.myCharacter.previous_exploded_color[0] = -1;
            }

            if (Minor_gem_explode())
            {
                //Debug.LogWarning("--------------Minor_gem_explode()");
                gems_useful_moved++;
                minorGemLatestSwitchInfo.n_gems_exploded++;

                Check_if_this_gem_have_the_same_color_of_the_gem_exploded_in_the_previous_move(board_array_master[main_gem_selected_x, main_gem_selected_y, 1]);

                List<Vector2Int> squareExplosions = new List<Vector2Int>();
                if (myRuleset.allow2x2Explosions)
                    squareExplosions = GetSquare2x2ExplosionInfoList(minor_gem_destination_to_x, minor_gem_destination_to_y, switchDiretion);//thisGemCan2x2Explode[new Vector2Int( minor_gem_destination_to_x, minor_gem_destination_to_y)].GetExplosionByDirection(switchDiretion);

                if (squareExplosions.Count > 0)
                {
                    //Debug.Log("minor 2 x 2  count: " + squareExplosions.Count);
                    minorGemLatestSwitchInfo.is2x2Explosion = true;
                }

                Detect_which_gems_will_explode(main_gem_selected_x, main_gem_selected_y, 1, squareExplosions);
            }
            else
            {
                //no color to keep track here
                activeCharacter.myCharacter.previous_exploded_color[1] = -1;
            }

            //keep track of score
            if (explode_same_color_again_with > 0)
                activeCharacter.myCharacter.explode_same_color_n_turn++;
            else
                activeCharacter.myCharacter.explode_same_color_n_turn = 0;


            Count_move();

            Empty_main_and_minor_gem_selections("SwitchingGems()");
        }
        else //this move is useless (no explosions)
        {
            //...but is a good move if move a bonus with free_switch
            if (myRuleset.trigger_by_select == Ruleset.trigger_by.free_switch &&
                ((board_array_master[main_gem_selected_x, main_gem_selected_y, 4] > 0) || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] > 0)))
            {
                Debug.Log("free switch without main explosion");
                bool badMove = false;

                //trigger bonus
                bool triggerMainBonus = false;
                bool triggerMinorBonus = false;
                if (board_array_master[main_gem_selected_x, main_gem_selected_y, 4] > 0)
                {
                    if (board_array_master[main_gem_selected_x, main_gem_selected_y, 4] != 7)//destroy_all_gem_with_this_color
                        {
                        triggerMainBonus = true;
                        //script_tiles_array[main_gem_selected_x, main_gem_selected_y].Trigger_bonus(false);
                        }
                    else
                    {
                        if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1] >= 0 && board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1] <= 6) //if is a gem
                        {
                            Annotate_explosions(main_gem_selected_x, main_gem_selected_y, ExplosionCause.bonus);
                            Destroy_all_gems_with_this_color(board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1]);
                        }
                        else
                            badMove = true;
                    }

                }

                if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] > 0)
                {
                    if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] != 7)//destroy_all_gem_with_this_color
                        {
                        triggerMinorBonus = true;
                        //script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].Trigger_bonus(false);
                        }
                    else
                    {
                        if (board_array_master[main_gem_selected_x, main_gem_selected_y, 1] >= 0 && board_array_master[main_gem_selected_x, main_gem_selected_y, 1] <= 6) //if is a gem
                        {
                            Annotate_explosions(minor_gem_destination_to_x, minor_gem_destination_to_y, ExplosionCause.bonus);
                            Destroy_all_gems_with_this_color(board_array_master[main_gem_selected_x, main_gem_selected_y, 1]);
                        }
                        else
                            badMove = true;
                    }

                }

                if (badMove)
                {
                    Debug.Log("badMove");
                    StartCoroutine(Start_bad_switch_animation());
                }
                else
                {
                    Move_gems_to_target_positions();

                    //trigger bonuese in the new inverted position
                    if (triggerMainBonus)
                        script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].Trigger_bonus(true);

                    if (triggerMinorBonus)
                        script_tiles_array[main_gem_selected_x, main_gem_selected_y].Trigger_bonus(true);

                    cameraController.Center_camera_to_move(main_gem_selected_x, main_gem_selected_y, _X_tiles, _Y_tiles,/* pivot_board,*/ globalRules.accuracy);
                    Empty_main_and_minor_gem_selections("Switch_gem A");
                }
            }
            else //is a bad move
            {
                Debug.Log("badMove");
                StartCoroutine(Start_bad_switch_animation());
            }
        }


        
        if (number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage_with_bonus > 0)//if there is something to explode
        {
            Cancell_hint();

            if (myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                Order_to_gems_to_explode();
        }
    }

    bool This_switch_will_produce_an_explosion()
    {
        //Debug.Log("This_switch_will_produce_an_explosion: ");

        //main gem
        if (board_array_master[main_gem_selected_x, main_gem_selected_y, 5] > 0)
        {
            if (temp_direction == 4 && board_array_master[main_gem_selected_x, main_gem_selected_y, 6] > 0)
            {
                //Debug.Log("main---------down");
                switchDiretion = MyDirections.Down;
                return true;
            }

            if (temp_direction == 5 && board_array_master[main_gem_selected_x, main_gem_selected_y, 7] > 0)
            {
                //Debug.Log("main---------up");
                switchDiretion = MyDirections.Up;
                return true;
            }

            if (temp_direction == 6 && board_array_master[main_gem_selected_x, main_gem_selected_y, 8] > 0)
            {
                //Debug.Log("main---------R");
                switchDiretion = MyDirections.Right;
                return true;
            }

            if (temp_direction == 7 && board_array_master[main_gem_selected_x, main_gem_selected_y, 9] > 0)
            {
                //Debug.Log("main---------L");
                switchDiretion = MyDirections.Left;
                return true;
            }
        }

        //minor gem
        if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 5] > 0)
        {
            if (temp_direction == 4 && board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 7] > 0)
            {
                //Debug.Log("minor--------- up");
                switchDiretion = MyDirections.Up;
                return true;
            }

            if (temp_direction == 5 && board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 6] > 0)
            {
                //Debug.Log("minor--------- down");
                switchDiretion = MyDirections.Down;
                return true;
            }

            if (temp_direction == 6 && board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 9] > 0)
            {
                //Debug.Log("minor--------- L");
                switchDiretion = MyDirections.Left;
                return true;
            }

            if (temp_direction == 7 && board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 8] > 0)
            {
                //Debug.Log("minor--------- R");
                switchDiretion = MyDirections.Right;
                return true;
            }
        }

        return false;
        /*
        if (((board_array_master[main_gem_selected_x, main_gem_selected_y, 5] > 0) || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 5] > 0)) &&

            (temp_direction == 4 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 6] > 0)
                          || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 7] > 0)))

            || (temp_direction == 5 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 7] > 0)
                            || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 6] > 0)))


            || (temp_direction == 6 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 8] > 0)
                             || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 9] > 0)))


            || (temp_direction == 7 && ((board_array_master[main_gem_selected_x, main_gem_selected_y, 9] > 0)
                             || (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 8] > 0)))
            )
            return true;
        else
            return false;
        */
    }

    bool Main_gem_explode()
    {
        if ((temp_direction == 4 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 6] > 0))//6 = up[n.how many gem explode if this gem go up]
                || (temp_direction == 5 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 7] > 0))//7 = down [n. how many gem explode if this gem go down]
                || (temp_direction == 6 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 8] > 0))//8 = right [n. how many gem explode if this gem go right]
                || (temp_direction == 7 && (board_array_master[main_gem_selected_x, main_gem_selected_y, 9] > 0))//9 = left [n. how many gem explode if this gem go left]
                )
        {
            return true;
        }
        else
            return false;
    }

    bool Minor_gem_explode()
    {
        if ((temp_direction == 4 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 7] > 0))//7 = down [n. how many gem explode if this gem go down]
             || (temp_direction == 5 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 6] > 0))//6 = up[n.how many gem explode if this gem go up]
             || (temp_direction == 6 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 9] > 0))//9 = left [n. how many gem explode if this gem go left]
             || (temp_direction == 7 && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 8] > 0))//8 = right [n. how many gem explode if this gem go right]
             )
            return true;
        else
            return false;
    }

    void Check_if_this_gem_have_the_same_color_of_the_gem_exploded_in_the_previous_move(int current_color)
    {
        if ((current_color == activeCharacter.myCharacter.previous_exploded_color[0]) //if this gem have the same color of the gem exploded in the previous move
            || (current_color == activeCharacter.myCharacter.previous_exploded_color[1]))
        {
            //Debug.Log("player explode same color with main gem");
            activeCharacter.myCharacter.previous_exploded_color[0] = current_color;
            explode_same_color_again_with = 1; //main gem
            if (myRuleset.gain_turn_if_explode_same_color_of_previous_move)
            {
                if ((!myRuleset.chain_turns_limit) || (activeCharacter.myCharacter.currentChainLenght < myRuleset.max_chain_turns))
                {
                    //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
                    Debug.Log("player gain a move with main gem - same color");
                    Gain_turns(myRuleset.move_gained_for_explode_same_color_in_two_adjacent_turn);
                }
            }
        }
        else
            activeCharacter.myCharacter.previous_exploded_color[0] = current_color;
        
    }

    void Detect_which_gems_will_explode(int __x, int __y, int _gem, List<Vector2Int> square2x2Explosions)//call from Switch_gem
    {
        //_gem 0 = main gem 
        //_gem 1 = minor gem 


        if (square2x2Explosions.Count > 0)
        {
            //print("SQUARE EXPLOSION " + _gem);

            if (_gem == 0)//main gem
                mainGemLatestSwitchInfo.n_gems_exploded = 4;
            else if (_gem == 1)//minor gem
                minorGemLatestSwitchInfo.n_gems_exploded = 4;

            for (int i = 0; i < square2x2Explosions.Count; i++)
                Annotate_explosions(square2x2Explosions[i].x, square2x2Explosions[i].y, ExplosionCause.switchingGems);

            Annotate_explosions(__x, __y, ExplosionCause.switchingGems);

        }
        else
        {
            #region vertical check
            //2 up and down
            if (((__y - 1) >= 0) && ((__y + 1) < _Y_tiles))
            {
                if ((board_array_master[__x, __y, 1] == board_array_master[__x, __y - 1, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y + 1, 1])//color match
                    /*&& board_array_master[__x, __y - 1, 11] == 0 && board_array_master[__x, __y + 1, 11] == 0*/)//gem are not busy
                {

                    position_of_gems_that_will_explode[_gem, 1] = true;
                    Annotate_explosions(__x, __y + 1, ExplosionCause.switchingGems);


                    position_of_gems_that_will_explode[_gem, 5] = true;
                    Annotate_explosions(__x, __y - 1, ExplosionCause.switchingGems);

                    //if there is one more down
                    if (((__y - 2) >= 0) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y - 2, 1])
                        /*&& board_array_master[__x, __y - 2, 11] == 0*/)//gem are not busy
                    {
                        position_of_gems_that_will_explode[_gem, 4] = true;
                        Annotate_explosions(__x, __y - 2, ExplosionCause.switchingGems);
                    }

                    //if there is one more up
                    if (((__y + 2) < _Y_tiles) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y + 2, 1])
                       /* && board_array_master[__x, __y + 2, 11] == 0*/)//gem are not busy
                    {
                        position_of_gems_that_will_explode[_gem, 0] = true;
                        Annotate_explosions(__x, __y + 2, ExplosionCause.switchingGems);
                    }

                }
            }
            //2 up
            if ((__y + 2) < _Y_tiles)
            {
                if ((board_array_master[__x, __y, 1] == board_array_master[__x, __y + 2, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y + 1, 1])
                   /* && board_array_master[__x, __y + 2, 11] == 0 && board_array_master[__x, __y + 1, 11] == 0*/)//gem are not busy
                {

                    position_of_gems_that_will_explode[_gem, 0] = true;
                    Annotate_explosions(__x, __y + 2, ExplosionCause.switchingGems);


                    position_of_gems_that_will_explode[_gem, 1] = true;
                    Annotate_explosions(__x, __y + 1, ExplosionCause.switchingGems);

                }
            }
            //2 down
            if ((__y - 2) >= 0)
            {
                if ((board_array_master[__x, __y, 1] == board_array_master[__x, __y - 2, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x, __y - 1, 1])
                   /* && board_array_master[__x, __y - 2, 11] == 0 && board_array_master[__x, __y - 1, 11] == 0*/)//gem are not busy)
                {

                    position_of_gems_that_will_explode[_gem, 5] = true;
                    Annotate_explosions(__x, __y - 1, ExplosionCause.switchingGems);

                    position_of_gems_that_will_explode[_gem, 4] = true;
                    Annotate_explosions(__x, __y - 2, ExplosionCause.switchingGems);

                }
            }
            #endregion

            #region horizontal check
            //2 right and left
            if (((__x - 1) >= 0) && ((__x + 1) < _X_tiles))
            {
                if ((board_array_master[__x, __y, 1] == board_array_master[__x - 1, __y, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x + 1, __y, 1])
                   /* && board_array_master[__x - 1, __y, 11] == 0 && board_array_master[__x + 1, __y, 11] == 0*/)//gem are not busy)
                {

                    position_of_gems_that_will_explode[_gem, 3] = true;
                    Annotate_explosions(__x + 1, __y, ExplosionCause.switchingGems);

                    position_of_gems_that_will_explode[_gem, 7] = true;
                    Annotate_explosions(__x - 1, __y, ExplosionCause.switchingGems);

                    //if there is one more at left
                    if (((__x - 2) >= 0) && (board_array_master[__x, __y, 1] == board_array_master[__x - 2, __y, 1])
                       /* && board_array_master[__x - 2, __y, 11] == 0*/)//gem are not busy)
                    {
                        position_of_gems_that_will_explode[_gem, 6] = true;
                        Annotate_explosions(__x - 2, __y, ExplosionCause.switchingGems);
                    }
                    //if there is one more at right
                    if (((__x + 2) < _X_tiles) && (board_array_master[__x, __y, 1] == board_array_master[__x + 2, __y, 1])
                       /* && board_array_master[__x + 2, __y, 11] == 0*/)//gem are not busy))
                    {
                        position_of_gems_that_will_explode[_gem, 2] = true;
                        Annotate_explosions(__x + 2, __y, ExplosionCause.switchingGems);
                    }
                }
            }
            //2 right
            if ((__x + 2) < _X_tiles)
            {
                if ((board_array_master[__x, __y, 1] == board_array_master[__x + 2, __y, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x + 1, __y, 1])
                   /* && board_array_master[__x + 2, __y, 11] == 0 && board_array_master[__x + 1, __y, 11] == 0*/)//gem are not busy)
                {

                    position_of_gems_that_will_explode[_gem, 3] = true;
                    Annotate_explosions(__x + 1, __y, ExplosionCause.switchingGems);

                    position_of_gems_that_will_explode[_gem, 2] = true;
                    Annotate_explosions(__x + 2, __y, ExplosionCause.switchingGems);

                }
            }
            //2 left
            if ((__x - 2) >= 0)
            {
                if ((board_array_master[__x, __y, 1] == board_array_master[__x - 2, __y, 1]) && (board_array_master[__x, __y, 1] == board_array_master[__x - 1, __y, 1])
                   /*  && board_array_master[__x - 2, __y, 11] == 0 && board_array_master[__x - 1, __y, 11] == 0*/)//gem are not busy)
                {

                    position_of_gems_that_will_explode[_gem, 7] = true;
                    Annotate_explosions(__x - 1, __y, ExplosionCause.switchingGems);

                    position_of_gems_that_will_explode[_gem, 6] = true;
                    Annotate_explosions(__x - 2, __y, ExplosionCause.switchingGems);

                }
            }
            #endregion

            //explode the gem moved
            if (_gem == 0) //main gem
            {
                //calculate score
                for (int i = 0; i < 8; i++)
                {
                    if (position_of_gems_that_will_explode[_gem, i])
                        mainGemLatestSwitchInfo.n_gems_exploded++;
                }
                Annotate_explosions(__x, __y, ExplosionCause.switchingGems);
            }
            if (_gem == 1) //minor gem
            {
                //calculate score
                for (int i = 0; i < 8; i++)
                {
                    if (position_of_gems_that_will_explode[_gem, i])
                        minorGemLatestSwitchInfo.n_gems_exploded++;
                }
                Annotate_explosions(__x, __y, ExplosionCause.switchingGems);
            }

    }

    //Debug.Log("n_gems_exploded_with_main_gem:" + mainGemLatestSwitchInfo.n_gems_exploded + "  n_gems_exploded_with_minor_gem:" + minorGemLatestSwitchInfo.n_gems_exploded);

        //if this is a big explosion
        if ((mainGemLatestSwitchInfo.n_gems_exploded > 3) || (minorGemLatestSwitchInfo.n_gems_exploded > 3))
        {



            if (myRuleset.gain_turn_if_explode_more_than_3_gems)
            {

                if ((!myRuleset.chain_turns_limit) || (activeCharacter.myCharacter.currentChainLenght < myRuleset.max_chain_turns))
                {
                    //Debug.Log("chain_turns_limit: " + chain_turns_limit + " * current_player_chain_lenght: " + current_player_chain_lenght + " * max_chain_turns: " + max_chain_turns);
                    if ((mainGemLatestSwitchInfo.n_gems_exploded == 4) || (minorGemLatestSwitchInfo.n_gems_exploded == 4))
                        Gain_turns(myRuleset.move_gained_when_explode_more_than_3_gems[0]);
                    else if ((mainGemLatestSwitchInfo.n_gems_exploded == 5) || (minorGemLatestSwitchInfo.n_gems_exploded == 5))
                        Gain_turns(myRuleset.move_gained_when_explode_more_than_3_gems[1]);
                    else if ((mainGemLatestSwitchInfo.n_gems_exploded == 6) || (minorGemLatestSwitchInfo.n_gems_exploded == 6))
                        Gain_turns(myRuleset.move_gained_when_explode_more_than_3_gems[2]);
                    else if ((mainGemLatestSwitchInfo.n_gems_exploded == 7) || (minorGemLatestSwitchInfo.n_gems_exploded == 7))
                        Gain_turns(myRuleset.move_gained_when_explode_more_than_3_gems[3]);

                }

            }

            //give bonus afther big explosion
            if (myRuleset.give_bonus_select == Ruleset.give_bonus.after_big_explosion)
            {
                //Debug.Log("give bonus afther big explosion... " + mainGemLatestSwitchInfo.n_gems_exploded);

                if ((_gem == 0) && (mainGemLatestSwitchInfo.n_gems_exploded > 3)) //check if main gem will become a bonus
                {
                    //print("Main gem...");
                    //Debug.Log("main gem " + minor_gem_destination_to_x  + "," + minor_gem_destination_to_y + " will become a bonus");
                    Assign_in_board_bonus(minor_gem_destination_to_x, minor_gem_destination_to_y, mainGemLatestSwitchInfo);
                }

                if ((_gem == 1) && (minorGemLatestSwitchInfo.n_gems_exploded > 3)) //check if minor gem will become a bonus
                {
                    //print("Minor gem...");
                    //Debug.Log("minor gem " + main_gem_selected_x + "," + main_gem_selected_y + " will become a bonus");
                    Assign_in_board_bonus(main_gem_selected_x, main_gem_selected_y, minorGemLatestSwitchInfo);
                }
            }

            if (myTheme.gem_explosion_fx_rule_selected == ThemeTemplate.gem_explosion_fx_rule.only_for_big_explosion)
            {
                if ((_gem == 0) && (mainGemLatestSwitchInfo.n_gems_exploded > 3)) //check if main gem will become a bonus
                {
                    script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].use_fx_big_explosion_here = mainGemLatestSwitchInfo.n_gems_exploded;
                }
                if ((_gem == 1) && (minorGemLatestSwitchInfo.n_gems_exploded > 3)) //check if minor gem will become a bonus
                {
                    script_tiles_array[main_gem_selected_x, main_gem_selected_y].use_fx_big_explosion_here = minorGemLatestSwitchInfo.n_gems_exploded;
                }
            }

        }

        //switch bonus
        if (((myRuleset.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem) || (myRuleset.trigger_by_select == Ruleset.trigger_by.free_switch))
            &&
            ((myRuleset.give_bonus_select == Ruleset.give_bonus.after_big_explosion) || (myRuleset.give_bonus_select == Ruleset.give_bonus.from_stage_file_or_from_gem_emitter)))
        {
            if ((_gem == 0) && (board_array_master[main_gem_selected_x, main_gem_selected_y, 4] > 0)) //if main gem explode and minor is a bonus
            {
                if (board_array_master[main_gem_selected_x, main_gem_selected_y, 4] != 7)//destroy_all_gem_with_this_color
                    {
                    script_tiles_array[main_gem_selected_x, main_gem_selected_y].Trigger_bonus(true);
                    }
                else
                {
                    Annotate_explosions(main_gem_selected_x, main_gem_selected_y, ExplosionCause.bonus);
                    Destroy_all_gems_with_this_color(board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1]);
                }
            }
            else if ((_gem == 1) && (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] > 0)) //if minor gem explode and main is a bonus
            {
                if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] != 7)//destroy_all_gem_with_this_color
                    {
                    script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].Trigger_bonus(true);
                    }
                else
                {
                    Annotate_explosions(minor_gem_destination_to_x, minor_gem_destination_to_y, ExplosionCause.bonus);
                    Destroy_all_gems_with_this_color(board_array_master[main_gem_selected_x, main_gem_selected_y, 1]);
                }
            }
        }

        //print(n_gems_exploded_with_main_gem + " ___ " + n_gems_exploded_with_minor_gem);
    }

    void Count_move()
    {
        activeCharacter.myCharacter.currentMovesLeft--;

        uIManager.Update_left_moves_text();

        if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
            CheckLose_player_have_zero_moves();
    }

    public void Update_turn_order_after_a_bad_move()
    {
        if (myRuleset.versus)
        {
            if (myRuleset.lose_turn_if_bad_move)
            {
                if (player_turn)
                {
                    player.myCharacter.currentMovesLeft--;
                    player.myCharacter.currentChainLenght = 0;
                    uIManager.Update_left_moves_text();

                    player_turn = false;
                    player_can_move = false;
                    Enemy_play();

                    player.myUI.gui_name_text.color = player.myUI.gui_color_off;
                    enemy.myUI.gui_name_text.color = enemy.myUI.gui_color_on;
                }
                else
                {
                    enemy.myCharacter.currentMovesLeft--;
                    enemy.myCharacter.currentChainLenght = 0;
                    uIManager.Update_left_moves_text();

                    player_turn = true;
                    player_can_move = true;

                    player.myUI.gui_name_text.color = player.myUI.gui_color_on;
                    enemy.myUI.gui_name_text.color = enemy.myUI.gui_color_off;
                }
            }
            else
            {
                player.myCharacter.currentChainLenght = 0;
                player_turn = true;
                player_can_move = true;
            }
        }
        else //solo
        {
            if (myRuleset.lose_turn_if_bad_move)
            {
                if (player_turn)
                {
                    player.myCharacter.currentMovesLeft--;
                    player.myCharacter.currentChainLenght = 0;
                    if (myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
                        CheckLose_player_have_zero_moves();
                }
                else
                {
                    enemy.myCharacter.currentMovesLeft--;
                    enemy.myCharacter.currentChainLenght = 0;
                }

                uIManager.Update_left_moves_text();
                if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
                {
                    if (player.myCharacter.currentMovesLeft > 0)
                        player_can_move = true;
                    else
                    {
                        UpdateTurn();
                    }
                }
                else
                    player_can_move = true;
            }
            else
            {
                player.myCharacter.currentChainLenght = 0;
                player_can_move = true;
            }
        }

        current_moveStatus = moveStatus.waitingNewMove;
    }




    void Move_gems_to_target_positions() //call from Switch_gem( or from Update_board_by_one_step(
    {
        Debug.Log("Move_gems_to_target_positions()");
        int temp_main_bonus = 0;
        int temp_minor_bonus = 0;


        //update color in array
        board_array_master[main_gem_selected_x, main_gem_selected_y, 1] = minor_gem_color;
        board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 1] = main_gem_color;

        //update bonus position
        temp_main_bonus = board_array_master[main_gem_selected_x, main_gem_selected_y, 4];
        temp_minor_bonus = board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4];
        board_array_master[main_gem_selected_x, main_gem_selected_y, 4] = temp_minor_bonus;
        board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 4] = temp_main_bonus;

        //update gem representation
        tile_C tile_script_main_gem = script_tiles_array[main_gem_selected_x, main_gem_selected_y];
        tile_C tile_script_minor_gem = script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y];


        tile_script_main_gem.myContent = avatar_minor_gem;

        tile_script_minor_gem.myContent = avatar_main_gem;


        StartCoroutine(tile_script_main_gem.Switch_gem_animation());
        StartCoroutine(tile_script_minor_gem.Switch_gem_animation());
    }

    void Empty_main_and_minor_gem_selections(string call_from)
    {
        if (board_array_master[main_gem_selected_x, main_gem_selected_y, 11] == 6)//if not exploded
            board_array_master[main_gem_selected_x, main_gem_selected_y, 11] = 0;

        if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] == 6)//if not exploded
            board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] = 0;

        //Debug.LogWarning("Empty_main_and_minor_gem_selections() " + call_from);// + player_can_move);

        isSwitching[main_gem_selected_x, main_gem_selected_y] = false;
        isSwitching[minor_gem_destination_to_x, minor_gem_destination_to_y] = false;

        main_gem_selected_x = -10;
        main_gem_selected_y = -10;
        avatar_main_gem = null;
        main_gem_color = -10;
        minor_gem_destination_to_x = -10;
        minor_gem_destination_to_y = -10;
        avatar_minor_gem = null;
        minor_gem_color = -10;

        current_moveStatus = moveStatus.waitingNewMove;
    }

 
    void BadSwitchTween()
    {
        float switchTime = 0.25f;

        //go
        LeanTween.move(avatar_main_gem.gameObject, script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, switchTime);
        LeanTween.move(avatar_minor_gem.gameObject, script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, switchTime);

        //go back
    }

    IEnumerator Start_bad_switch_animation()
    {


        //Debug.Log("Start_bad_switch_animation() " + player_can_move + " minor: " + minor_gem_destination_to_x + "," + minor_gem_destination_to_y + " main: " + main_gem_selected_x + "," + main_gem_selected_y);
        //Debug.Log(board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] + " ... " + board_array_master[main_gem_selected_x, main_gem_selected_y, 11]);


        float switchTime = 0.25f;

        //go to wrong position
        LeanTween.move(avatar_main_gem.gameObject, script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, switchTime);
        LeanTween.move(avatar_minor_gem.gameObject, script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, switchTime);


        //reach bad position
        yield return new WaitForSeconds(switchTime);
        audioManager.Play_sfx(audioManager.bad_move_sfx);

        //return to original position
        LeanTween.move(avatar_main_gem.gameObject, script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, switchTime);
        LeanTween.move(avatar_minor_gem.gameObject, script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, switchTime);

        yield return new WaitForSeconds(switchTime);

        /*
        while (Vector3.Distance(script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, avatar_main_gem.transform.position) > globalRules.accuracy)
        {
            Debug.Log("while a" + " minor: " + minor_gem_destination_to_x + "," + minor_gem_destination_to_y +  " = " + board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] +
                " main: " + main_gem_selected_x + "," + main_gem_selected_y + " = " + board_array_master[main_gem_selected_x, main_gem_selected_y, 11]);
            if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] != 6 || board_array_master[main_gem_selected_x, main_gem_selected_y, 11] != 6)
            {
                Debug.LogError("go..." + minor_gem_destination_to_x + "," + minor_gem_destination_to_y + " = " +  board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] 
                                + " ... " + main_gem_selected_x + "," + main_gem_selected_y + " = " + board_array_master[main_gem_selected_x, main_gem_selected_y, 11]);

                Debug.Break();

                yield return null;
            }

            //go to wrong position
            avatar_main_gem.transform.Translate(((script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position - avatar_main_gem.transform.position).normalized) * globalRules.falling_speed * Time.deltaTime, Space.World);
            avatar_minor_gem.transform.Translate(((script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position - avatar_minor_gem.transform.position).normalized) * globalRules.falling_speed * Time.deltaTime, Space.World);


            yield return new WaitForEndOfFrame();
        }
        //reach bad position
        if (Vector3.Distance(script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, avatar_main_gem.transform.position) != 0)
            avatar_main_gem.transform.position = script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position;

        if (Vector3.Distance(script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, avatar_minor_gem.transform.position) != 0)
            avatar_minor_gem.transform.position = script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position;

        audioManager.Play_sfx(audioManager.bad_move_sfx);


        //return to original position
        while (Vector3.Distance(script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, avatar_main_gem.transform.position) > globalRules.accuracy)
        {
            Debug.Log("while b" + " minor: " + minor_gem_destination_to_x + "," + minor_gem_destination_to_y + " = " + board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] +
                " main: " + main_gem_selected_x + "," + main_gem_selected_y + " = " + board_array_master[main_gem_selected_x, main_gem_selected_y, 11]);
            if (board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11] != 6 || board_array_master[main_gem_selected_x, main_gem_selected_y, 11] != 6)
            {
                Debug.LogError("go..." + minor_gem_destination_to_x + "," + minor_gem_destination_to_y + " = " + board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11]
                                + " ... " + main_gem_selected_x + "," + main_gem_selected_y + " = " + board_array_master[main_gem_selected_x, main_gem_selected_y, 11]);
                Debug.Break();

                yield return null;
            }

            avatar_main_gem.transform.Translate(((script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position - avatar_main_gem.transform.position).normalized) * globalRules.switch_speed * Time.deltaTime, Space.World);
            avatar_minor_gem.transform.Translate(((script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position - avatar_minor_gem.transform.position).normalized) * globalRules.switch_speed * Time.deltaTime, Space.World);
            yield return new WaitForEndOfFrame();
        }
        */

        //reach original position
            if (Vector3.Distance(script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, avatar_main_gem.transform.position) != 0)
                avatar_main_gem.transform.position = script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position;

            if (Vector3.Distance(script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, avatar_minor_gem.transform.position) != 0)
                avatar_minor_gem.transform.position = script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position;

            
            Update_turn_order_after_a_bad_move();
            Empty_main_and_minor_gem_selections("Switch_gem B");
            //Debug.Log("End_bad_switch_animation() " + player_can_move);
            temp_direction = -1;
        


        /*
        if (Vector3.Distance(script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, avatar_main_gem.transform.position) > globalRules.accuracy)
        {
            Debug.Log("A");
            yield return new WaitForSeconds(0.015f);

            avatar_main_gem.transform.Translate(((script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position - avatar_main_gem.transform.position).normalized) * globalRules.falling_speed * Time.deltaTime, Space.World);
            avatar_minor_gem.transform.Translate(((script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position - avatar_minor_gem.transform.position).normalized) * globalRules.falling_speed * Time.deltaTime, Space.World);


            StartCoroutine(Start_bad_switch_animation());
        }
        else
        {
            Debug.Log("B");
            if (Vector3.Distance(script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, avatar_main_gem.transform.position) != 0)
                avatar_main_gem.transform.position = script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position;

            if (Vector3.Distance(script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, avatar_minor_gem.transform.position) != 0)
                avatar_minor_gem.transform.position = script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position;

            audioManager.Play_sfx(audioManager.bad_move_sfx);
            StartCoroutine(End_bad_switch_animation());

            yield return null;
        }
        */
    }


    /*
    IEnumerator End_bad_switch_animation()
    {
        Debug.Log("End_bad_switch_animation " + main_gem_selected_x + "," + main_gem_selected_y + minor_gem_destination_to_x + "," + minor_gem_destination_to_y);
        Debug.Log(board_array_master[minor_gem_destination_to_x, minor_gem_destination_to_y, 11]);
        Debug.Log(board_array_master[main_gem_selected_x, main_gem_selected_y, 11]);

        if (Vector3.Distance(script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, avatar_main_gem.transform.position) > globalRules.accuracy)
        {

            avatar_main_gem.transform.Translate(((script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position - avatar_main_gem.transform.position).normalized) * globalRules.switch_speed * Time.deltaTime, Space.World);
            avatar_minor_gem.transform.Translate(((script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position - avatar_minor_gem.transform.position).normalized) * globalRules.switch_speed * Time.deltaTime, Space.World);
            yield return new WaitForSeconds(0.015f);
            StartCoroutine(End_bad_switch_animation());
        }
        else
        {

            if (Vector3.Distance(script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position, avatar_main_gem.transform.position) != 0)
                avatar_main_gem.transform.position = script_tiles_array[main_gem_selected_x, main_gem_selected_y].transform.position;

            if (Vector3.Distance(script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position, avatar_minor_gem.transform.position) != 0)
                avatar_minor_gem.transform.position = script_tiles_array[minor_gem_destination_to_x, minor_gem_destination_to_y].transform.position;

            Empty_main_and_minor_gem_selections("Switch_gem");
            Update_turn_order_after_a_bad_move();
            Debug.Log("End_bad_switch_animation() " + player_can_move);
            temp_direction = -1;
            yield return null;
        }

    }*/

    int primary_explosion_count;
    public void Execute_primary_explosions()
    {
        //print("Execute_primary_explosions " + reserved_for_primary_explosion.Count);

        foreach (tile_C reserved in reserved_for_primary_explosion)
            board_array_master[reserved._x, reserved._y, 11] = 666;

        primary_explosion_count += reserved_for_primary_explosion.Count;

        reserved_for_primary_explosion.Clear();

    }

}
