using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class Board_C : MonoBehaviour {


    public tile_C[,] script_tiles_array;

    [HideInInspector] public int _X_tiles;
    [HideInInspector] public int _Y_tiles;

    [HideInInspector] public int number_of_tiles_leader;
    [HideInInspector] public tile_C[] tiles_leader_array;
    [HideInInspector] public int total_tiles;
    [HideInInspector] public List<tile_C> immovable_elements;
    [HideInInspector] public List<tile_C> elements_to_damage_list;
    [HideInInspector] public tile_C[,] bottom_tiles_array;//[board orientation, tile]this help to check when token and junk bust exit from the board 
    [HideInInspector] public int[] number_of_bottom_tiles;//[board orientation]
    [HideInInspector] public int current_board_orientation;

    //public Sprite[] lock_gem_hp;
    [HideInInspector] public int padlock_count;
    [HideInInspector] public int ice_count;

    //public Sprite[] block_hp;
    [HideInInspector] public int block_count;

    //debug
    //public List<TileContent> tileContents;
    //

    public bool[,] isSwitching;
    public int[,,] board_array_master;//this keep track of the status of every tiles, gems, lock and so on... in the board
                                      /* 0 = tile [-1 = "no tile"; 0 = "hp = 0"; 1 = "hp 1"...]
                                       * 1 = gem [-99 = no gem; 
                                       * 			from 0 to 6 color gem; 
                                       * 				7 = special explosion good 
                                       * 				8 = special explosion bad 
                                       * 				9 = neutre = it don't explode when 3 in a row
                                       * 			10 = random gem; 
                                       * 			20 = ?; 
                                       * 			30 = ?; 
                                       * 			
                                       * 			40 = immune block
                                       * 			
                                       * 			41 = block hp 1
                                       * 			42 = block hp 2
                                       * 			43 = block hp 3
                                       * 			
                                       * 			51 = falling block hp 1
                                       * 			52 = falling block hp 2
                                       * 			53 = falling block hp 3
                                       * 			
                                       * 			61 = generative block hp 1
                                       * 			62 = generative block hp 2
                                       * 			63 = generative block hp 3

                                       * 			64 = ?
                                       * 			70 = ?
                                       * 			71 = ?
                                       * 			72 = ?
                                       * 			73 = ?
                                       * 			74 = ?
                                       * 2 = special tile:
                                       * 		0 = no special
                                       * 		1 = start
                                       * 		2 = goal
                                       * 		3 = path
                                       * 		10 = door a
                                       * 		11 = door b
                                       * 		12 = door c
                                       * 		13 = door d
                                       * 		14 = door e
                                       * 		20 = item a
                                       * 		21 = item b
                                       * 		22 = item c
                                       * 		23 = item d
                                       * 		24 = item e
                                       * 3 = restraint [0 = no padlock; 1 = padlock, 2 = ice, 3 = falling padlock, 4 = cage (gem fall and cage stay in position prewenting swap)
                                       * 
                                       * 4 = special [-200 = token
                                       * 				-100 = junk
                                       * 				0 = no
                                       * 				1= destroy_one
                                       * 				2= Switch_gem_teleport
                                       * 				3= bomb
                                       * 				4= horiz
                                       * 				5= vertic
                                       * 				6= horiz_and_vertic
                                       * 				7= destroy_all_same_color
                                       * 
                                       * 				8= more_time
                                       * 				9= more_moves
                                       * 				10= more_hp
                                       * 				11 = damage opponent
                                       * 				
                                       * 				12= rotate_board_L
                                       * 				13= rotate_board_R
                                       * 				14 = destroy single random gems (meteor shower)
                                       * 
                                       * 				100 = time bomb
                                       * 
                                       * 
                                       * 5 = number of useful moves of this gem [from 0 = none, to 4 = all directions]
                                              6 = up [n. how many gem explode if this gem go up]
                                              7 = down [n. how many gem explode if this gem go down]
                                              8 = right [n. how many gem explode if this gem go right]
                                              9 = left [n. how many gem explode if this gem go left]
                                          10 = this thing can fall (0=false;1=true) (2 = explode if reach board bottom border)
                                          11 = current tile action in progress (0=none; 1=explosion;                    2=creation;                         3=falling down;4=falling down R;5=falling down L;   6 = Switching this gem)
                                                                                        111 = explosion ongoing         222= creation ongoing               333 = falling animation ongoing                     66 = primary explosion when switching end
                                                                                                                                                                                                                666 = primary explosion
                                                                                                                                                                                                                
                                                                                        
                                          12 = this tile generate gems (0= no; 1= yes; 2= yes and it is activated)
                                          13 = tile already checked (0=false;1=true)
                                          14 = block_hp
                                          15 = restrain hp
                                       */
                                      //int board_array_master_length = 15;



    public void Show_all_token_on_board()//call from Create_new_board(), tile_C.Update_tile_hp()
    {
        if (!token_showed)
        {
            token_showed = true;

            for (int y = 0; y < _Y_tiles; y++)
            {
                for (int x = 0; x < _X_tiles; x++)
                {
                    if (token_place_card[x, y])
                    {
                        if (board_array_master[x, y, 11] != 0)
                            Debug.LogWarning(x + "," + y + " Token problem: " + board_array_master[x, y, 11]);

                        bool abortedExplosion = false;
                        if (board_array_master[x, y, 11] == 1)//problem this gem is exploding
                        {
                            //abort explosion
                            script_tiles_array[x, y].StopAllCoroutines();
                            script_tiles_array[x, y].myContent.ResetAvatarTranform();

                            if (board_array_master[x, y, 11] == 6)
                                Debug.LogError("6");

                            board_array_master[x, y, 11] = 0;//destruction is over
                            abortedExplosion = true;
                        }

                        board_array_master[x, y, 1] = 9;
                        board_array_master[x, y, 4] = -200;
                        script_tiles_array[x, y].StartCoroutine(script_tiles_array[x, y].Show_token(abortedExplosion));
                    }
                }
            }
        }
    }



}
