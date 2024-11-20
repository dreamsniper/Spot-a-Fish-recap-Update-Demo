using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour
{

    [HideInInspector] public Board_C board;
    [HideInInspector] public explosion_score explosion_score_script;
    [HideInInspector] public int use_fx_big_explosion_here;

    //bonus fx limits
    int fx_end_r;
    int fx_end_l;
    int fx_end_up;
    int fx_end_down;

    [HideInInspector] public int after_explosion_I_will_become_this_bonus; // 0 = no bonus

    [HideInInspector] public int _x;
    [HideInInspector] public int _y;
    int move_direction;

    int[] two_last_colors_created = { 0, 1 };//this avoid to create 3 gems with the same color
    int random_color;

    public SpriteRenderer sprite_hp;
    /*[HideInInspector]*/public TileContent myContent;
    public GameObject my_padlock;
    public GameObject my_ice;



    void Start()
    {
        /*
        if (board.menuKitBridge)
            board.menuKitBridge.Start_menuKit();*/

        explosion_score_script = board.uIManager.the_gui_score_of_this_move.GetChild(0).GetComponent<explosion_score>();
    }

    public void Debug_show_available_moves(int show_this_move)
    {
        if (show_this_move == 3)
            GetComponent<Renderer>().material.color = Color.gray;
        else if (show_this_move > 3)
            GetComponent<Renderer>().material.color = Color.black;
        else
            GetComponent<Renderer>().material.color = Color.white;
        /*
		if (board.board_array_master[_x,_y,0] >3) //if this gem can make an explosive move
			{
			renderer.material.color = Color.black;
			}
		else
			renderer.material.color = Color.white;*/
    }

    public void Debug_show_my_color()
    {
        if (board.board_array_master[_x, _y, 1] == -99)
            GetComponent<Renderer>().material.color = Color.gray;
        else if (board.board_array_master[_x, _y, 1] == 0)
            GetComponent<Renderer>().material.color = Color.red;
        else if (board.board_array_master[_x, _y, 1] == 1)
            GetComponent<Renderer>().material.color = Color.cyan;
        else if (board.board_array_master[_x, _y, 1] == 2)
            GetComponent<Renderer>().material.color = Color.magenta;
        else if (board.board_array_master[_x, _y, 1] == 3)
            GetComponent<Renderer>().material.color = Color.yellow;
    }


    #region interaction



    public void MyOnMouseDown()
    {
        if (board.game_end || board.waitForDelay)
            return;

        board.touch_number++;

        //Debug.Log("board.touch_number " + board.touch_number);
        //Debug.Log("Type: " + board.board_array_master[_x, _y, 1] +  "    HP " + board.board_array_master[_x, _y, 15] + " restraint: " + board.board_array_master[_x, _y, 3] + " can fall: " + board.board_array_master[_x, _y, 10] + " status: " + board.board_array_master[_x, _y, 11]);
       /*Debug.Log(_x + "," + _y

      + "** 5 = " + board.board_array_master[_x, _y, 5]
      + " 2x2 R>" + board.thisGemCan2x2Explode[_x, _y].GetExplosionByDirection(Board_C.MovementDiretion.Right) 
      + " L " + board.thisGemCan2x2Explode[_x, _y].GetExplosionByDirection(Board_C.MovementDiretion.Left)
      + " UP " + board.thisGemCan2x2Explode[_x, _y].GetExplosionByDirection(Board_C.MovementDiretion.Up)
      + " Down " + board.thisGemCan2x2Explode[_x, _y].GetExplosionByDirection(Board_C.MovementDiretion.Down)
      );
        /*
        /*
        Debug.Log(_x + "," + _y
             + "** 1 = " + board.board_array_master[_x, _y, 1]
             + "** 2 = " + board.board_array_master[_x, _y, 2]
             + "** 3 = " + board.board_array_master[_x, _y, 3]
             + "** 4 = " + board.board_array_master[_x, _y, 4]
             + "** 5 = " + board.board_array_master[_x, _y, 5]
             + "** 6 = " + board.board_array_master[_x, _y, 6]
             + "** 7 = " + board.board_array_master[_x, _y, 7]
             + "** 8 = " + board.board_array_master[_x, _y, 8]
             + "** 9 = " + board.board_array_master[_x, _y, 9]
             + "** 10 = " + board.board_array_master[_x, _y, 10]
             + "** 11 = " + board.board_array_master[_x, _y, 11]
             + "** 12 = " + board.board_array_master[_x, _y, 12]
             + "** 13 = " + board.board_array_master[_x, _y, 13]
             + "** 14 = " + board.board_array_master[_x, _y, 14]
             // + "** 15 = " + board.board_array_master[_x,_y,15]
             // + "** 16 = " + board.board_array_master[_x,_y,16]
             //+ "** 17 = " + board.board_array_master[_x,_y,17]
             );
        */

        if (board.touch_number == 1)
        {
            board.cursor.position = this.transform.position;

           // Debug.LogWarning(_x + "," + _y + " = " + board.board_array_master[_x, _y, 5]);

            /*
            Debug.Log( "** 5 = " + board.board_array_master[_x, _y, 5]
                + "** 6 down = " + board.board_array_master[_x, _y, 6]
                + "** 7 up = " + board.board_array_master[_x, _y, 7]
                + "** 8 right = " + board.board_array_master[_x, _y, 8]
                + "** 9 left = " + board.board_array_master[_x, _y, 9]
                ); */
            //Debug.Log("state " + board.board_array_master[_x, _y, 11] + " - check? " + board.board_array_master[_x, _y, 13]);
            /*
            Debug.Log("kind " + board.board_array_master[_x, _y, 1]
                     + " useful moves " + board.board_array_master[_x, _y, 5]);

            
			Debug.Log(
				"tile hp " + board.board_array_master[_x,_y,0]
				+ "kind " + board.board_array_master[_x,_y,1]
				+ "block hp " + board.board_array_master[_x,_y,14]
				+ "** 5 = " + board.board_array_master[_x,_y,5]
				+ "** 6 down = " + board.board_array_master[_x,_y,6]
				+ "** 7 up = " + board.board_array_master[_x,_y,7]
				+ "** 8 right = " + board.board_array_master[_x,_y,8]
				+ "** 9 left = " + board.board_array_master[_x,_y,9]
				);*/

            /*
				Board_C debug = (Board_C)boardthis_board.GetComponent("Board_C");
				if (debug.debug)//change gem color when click over it
				{
					if (boardarray_master[_x,_y,1] +1 == board.gem_length)
						boardarray_master[_x,_y,1]  = 0;
					else
						boardarray_master[_x,_y,1] ++;

					SpriteRenderer sprite_gem = my_gem.GetComponent<SpriteRenderer>();
						sprite_gem.sprite = board.gem_colors[boardarray_master[_x,_y,1]];


				}
				else
				{
                */
            /*
            Debug.Log(_x + "," + _y 
                        + " " + board.isSwitching[_x, _y]
                      + "** 1 = " + board.board_array_master[_x,_y,1] 
                      + "** 2 = " + board.board_array_master[_x,_y,2]
                      + "** 3 = " + board.board_array_master[_x,_y,3]
                      + "** 4 = " + board.board_array_master[_x,_y,4]
                      + "** 5 = " + board.board_array_master[_x,_y,5]
                      + "** 6 = " + board.board_array_master[_x,_y,6]
                      + "** 7 = " + board.board_array_master[_x,_y,7]
                      + "** 8 = " + board.board_array_master[_x,_y,8]
                      + "** 9 = " + board.board_array_master[_x,_y,9]
                      + "** 10 = " + board.board_array_master[_x,_y,10]
                      + "** 11 = " + board.board_array_master[_x,_y,11]
                      + "** 12 = " + board.board_array_master[_x,_y,12]
                      + "** 13 = " + board.board_array_master[_x,_y,13]
                      + "** 14 = " + board.board_array_master[_x,_y,14]
                     // + "** 15 = " + board.board_array_master[_x,_y,15]
                     // + "** 16 = " + board.board_array_master[_x,_y,16]
                      //+ "** 17 = " + board.board_array_master[_x,_y,17]
                      );
*/
            //Debug.Log(_x + "," + _y  + " = special: " + board.board_array_master[_x, _y, 4] + " status: " + board.board_array_master[_x, _y, 11]);
            //}

            if (board.board_array_master[_x, _y, 11] != 0 || board.isSwitching[_x, _y] || board.current_moveStatus != Board_C.moveStatus.waitingNewMove)//this gem is busy, you can't use it now
                return;


            if (board.player_turn && (board.player_can_move || board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime))
            {

                board.n_combo = 0;//interact break the combo
                if (board.bonus_select == Bonus.None)
                {
                    if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) //if this is a gem
                            && (board.board_array_master[_x, _y, 3] == 0)) //no padlock
                    {
                        if ((board.myRuleset.give_bonus_select != Ruleset.give_bonus.no &&board.myRuleset.trigger_by_select == Ruleset.trigger_by.click) && (board.board_array_master[_x, _y, 1] == 9) && (board.board_array_master[_x, _y, 4] > 0))// click on a clickable bonus
                        {
                            Debug.Log("click bonus");
                            Cancell_old_selection();
                            
                            Trigger_bonus(true);

                            return;
                        }

                        if (board.main_gem_selected_x < 0)//if none gem is selected
                        {
                            I_become_main_gem();
                        }
                        else if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y == _y))
                        {
                            //you have click on the gem already selected
                        }
                        else if ((board.main_gem_selected_x - 1 == _x) && (board.main_gem_selected_y == _y))
                        {
                            //click on the gem next left of the main gem
                            if (board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 11] == 0)
                                I_become_minor_gem(7);
                            else
                                CancelMainGemSelection();
                        }
                        else if ((board.main_gem_selected_x + 1 == _x) && (board.main_gem_selected_y == _y))
                        {
                            //click on the gem next right of main gem
                            if (board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 11] == 0)
                                I_become_minor_gem(6);
                            else
                                CancelMainGemSelection();
                        }
                        else if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y - 1 == _y))
                        {
                            //click on the gem next up main gem
                            if (board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 11] == 0)
                                I_become_minor_gem(5);
                            else
                                CancelMainGemSelection();
                        }
                        else if ((board.main_gem_selected_x == _x) && (board.main_gem_selected_y + 1 == _y))
                        {
                            //click on the gem next down main gem
                            if (board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 11] == 0)
                                I_become_minor_gem(4);
                            else
                                CancelMainGemSelection();
                        }
                        else //click on a gem not adjacent the gem already selected, so this gem will be the new main gem
                        {
                            CancelMainGemSelection();
                            I_become_main_gem();
                        }
                    }
                    else
                        Cancell_old_selection();
                }
                else
                {
                    Cancell_old_selection();
                    Try_to_use_bonus_on_this_tile();
                }
            }

        }
    }

    public void MyOnMouseEnter()
    {
        if (board.player_turn && (board.player_can_move || board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime))
        {
            if (board.touch_number == 0)
            {
                board.cursor.position = this.transform.position;
                board.cursor.gameObject.SetActive(true);
            }

        }

        if (board.touch_number == 1)
            Gem_drag();

    }

    public void MyOnMouseExit()
    {

        if (board.player_turn && (board.player_can_move || board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime))
        {
            if (board.touch_number == 0)
                board.cursor.gameObject.SetActive(false);
        }

    }


    void Cancell_old_selection()
    {
        if (board.current_moveStatus != Board_C.moveStatus.waitingNewMove)
            return;

        Debug.Log("Cancell_old_selection");

        if (board.bonus_select != Bonus.SwitchGemTeleport)
        {
            if (board.main_gem_selected_x >= 0 && board.main_gem_selected_y >= 0)
                board.isSwitching[board.main_gem_selected_x, board.main_gem_selected_y] = false;

            if (board.minor_gem_destination_to_x >= 0 && board.minor_gem_destination_to_y >= 0)
                board.isSwitching[board.minor_gem_destination_to_x, board.minor_gem_destination_to_y] = false;

            board.main_gem_selected_x = -10;
            board.main_gem_selected_y = -10;
            board.main_gem_color = -10;
            board.minor_gem_destination_to_x = -10;
            board.minor_gem_destination_to_y = -10;
            board.minor_gem_color = -10;
        }
    }


    void Gem_drag()
    {
        print("try Gem_drag");

        /*
        Debug.Log(_x + "," + _y
                     + "** 1 = " + board.board_array_master[_x, _y, 1]
                     + "** 2 = " + board.board_array_master[_x, _y, 2]
                     + "** 3 = " + board.board_array_master[_x, _y, 3]
                     + "** 4 = " + board.board_array_master[_x, _y, 4]
                     + "** 5 = " + board.board_array_master[_x, _y, 5]
                     + "** 6 = " + board.board_array_master[_x, _y, 6]
                     + "** 7 = " + board.board_array_master[_x, _y, 7]
                     + "** 8 = " + board.board_array_master[_x, _y, 8]
                     + "** 9 = " + board.board_array_master[_x, _y, 9]
                     + "** 10 = " + board.board_array_master[_x, _y, 10]
                     + "** 11 = " + board.board_array_master[_x, _y, 11]
                     + "** 12 = " + board.board_array_master[_x, _y, 12]
                     + "** 13 = " + board.board_array_master[_x, _y, 13]
                     + "** 14 = " + board.board_array_master[_x, _y, 14]
                     // + "** 15 = " + board.board_array_master[_x,_y,15]
                     // + "** 16 = " + board.board_array_master[_x,_y,16]
                     //+ "** 17 = " + board.board_array_master[_x,_y,17]
                     );
        */

        if (board.current_moveStatus != Board_C.moveStatus.waitingNewMove || board.isSwitching[_x, _y])
        {
            Debug.Log("invalid drag - SwitchingGems busy");
            return;
        }

        if (board.board_array_master[_x, _y, 11] != 0 && board.current_moveStatus == Board_C.moveStatus.waitingNewMove) //this gem is busy, you can't selet it now
            {
            Debug.Log("invalid drag - gem busy");
            CancelMainGemSelection();
            return;
            }

        if (board.touch_number == 1 && (board.bonus_select == Bonus.None))
        {
            if (board.avatar_main_gem && board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 11] == 0 && (board.player_can_move || board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime ))
            {
                //Debug.Log("Gem_drag() " + board.player_can_move);
                if ((board.board_array_master[_x, _y, 1] >= 0) && (board.board_array_master[_x, _y, 1] <= 9) //it is a gem
                    && (board.board_array_master[_x, _y, 3] == 0)) //and it is free
                {
                    if ((board.main_gem_selected_x == _x + 1) && (board.main_gem_selected_y == _y)) //move left
                        I_become_minor_gem(7);
                    else if ((board.main_gem_selected_x == _x - 1) && (board.main_gem_selected_y == _y)) //more right
                        I_become_minor_gem(6);
                    else if ((board.main_gem_selected_y == _y + 1) && (board.main_gem_selected_x == _x)) //move up
                        I_become_minor_gem(5);
                    else if ((board.main_gem_selected_y == _y - 1) && (board.main_gem_selected_x == _x)) //move down
                        I_become_minor_gem(4);
                }
                else
                {
                    Debug.LogWarning("invalid drag");
                    CancelMainGemSelection();
                }
            }
        }
    }

    void CancelMainGemSelection()
    {
        if(board.main_gem_selected_x >= 0 && board.main_gem_selected_y >= 0)
            board.isSwitching[board.main_gem_selected_x, board.main_gem_selected_y] = false;

        board.main_gem_selected_x = -10;
        board.main_gem_selected_y = -10;
        board.avatar_main_gem = null;
        board.main_gem_color = -10;
    }


    public void I_become_main_gem()
    {
        if (board.current_moveStatus != Board_C.moveStatus.waitingNewMove)
        {
            Debug.LogWarning("can't store - I_become_main_gem - now");
            return;
        }

        if (board.board_array_master[_x, _y, 11] != 0 || board.isSwitching[_x, _y]) //this gem is busy, you can't selet it now
            return;

        board.isSwitching[_x, _y] = true;
        board.isSwitching[_x, _y] = true;

        //Debug.Log("I_become_main_gem() " + _x+","+_y);
        board.avatar_main_gem = myContent;
        board.main_gem_selected_x = _x;
        board.main_gem_selected_y = _y;
        board.main_gem_color = board.board_array_master[_x, _y, 1];

        if (!board.player_turn)
            board.cursor.gameObject.SetActive(true);

        //empty old selection
        board.minor_gem_destination_to_x = -10;
        board.minor_gem_destination_to_y = -10;
        board.minor_gem_color = -10;
        board.avatar_minor_gem = null;
    }

    public void I_become_minor_gem(int direction)
    {
        if (board.current_moveStatus != Board_C.moveStatus.waitingNewMove)
        {
            //Debug.LogWarning("can't store - I_become_minor_gem -  move now");
            return;
        }

        if (board.board_array_master[_x, _y, 11] != 0 || board.isSwitching[_x, _y]) //this gem is busy, you can't selet it now
            return;

        board.isSwitching[_x, _y] = true;

        //Debug.Log("I_become_minor_gem() " + _x+","+_y);
        board.avatar_minor_gem = myContent;
        board.minor_gem_destination_to_x = _x;
        board.minor_gem_destination_to_y = _y;
        board.minor_gem_color = board.board_array_master[_x, _y, 1];

       
        if (board.player_turn)
        {
            board.cursor.position = this.transform.position;

            //if (board.player_can_move_when_gem_falling)
                //{
                board.temp_direction = direction;
                board.current_moveStatus = Board_C.moveStatus.switchingGems;
                if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                    board.SwitchingGems();

                //}
                //else
                //board.Switch_gem(direction, "tile.I_become_minor_gem");
        }
        else
        {
            board.waitForDelay = true;
            move_direction = direction;
            Invoke("Delay_switch", board.globalRules.enemy_move_delay);
        }
    }

    void Delay_switch()
    {
        //be sure that the gems are still free to use
        if (board.board_array_master[board.main_gem_selected_x, board.main_gem_selected_y, 11] != 0 
            || board.board_array_master[_x, _y, 11] != 0)
        {
            board.isSwitching[_x, _y] = false;
            CancelMainGemSelection();
            Cancell_old_selection();

            return;
        }

        board.waitForDelay = false;


        board.cursor.position = this.transform.position;
        //board.Switch_gem(move_direction, "tile.I_become_minor_gem");
        board.temp_direction = move_direction;
        board.current_moveStatus = Board_C.moveStatus.switchingGems;
        if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
            board.SwitchingGems();
    }

    #endregion



}
