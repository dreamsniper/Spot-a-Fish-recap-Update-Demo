using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour {


    #region destroy gem

    public void Check_the_content_of_this_tile()
    {
        //Debug.Log("Check_the_content_of_this_tile() " + _x + " , " + _y + " = " + board.board_array_master[_x, _y, 4]);
        if (board.board_array_master[_x, _y, 4] == -100)
        {
            //Debug.Log("junk in " + _x + "," +_y);
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.secondayExplosion);
        }
        else if (board.board_array_master[_x, _y, 4] == -200)
        {
            //Debug.Log("-----token in " + _x + "," +_y);
            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.secondayExplosion);
            board.number_of_token_collected++;
            board.uIManager.Update_token_count();
        }
        else if ((board.board_array_master[_x, _y, 4] > 0) && (board.myRuleset.trigger_by_select == Ruleset.trigger_by.inventory))
        {

            if (board.board_array_master[_x, _y, 11] == 333)
            {
                Debug.Log(" this is need for reset the 333 status in player_can_move_when_gem_falling " + _x + "," + _y);
                board.board_array_master[_x, _y, 11] = 0;//this is need for reset the 333 status in player_can_move_when_gem_falling
            }

            //add bonus to inventory
            board.uIManager.Update_inventory_bonus(board.board_array_master[_x, _y, 4], +1);

            board.board_array_master[_x, _y, 4] = 0;//deactivate bonus to avoid trigger when explode
            board.Update_on_board_bonus_count();
            after_explosion_I_will_become_this_bonus = 0;

            board.Annotate_explosions(_x, _y, Board_C.ExplosionCause.secondayExplosion);

        }
    }


    public void Explosion()
    {
        //Debug.Log(_x + "," + _y + " Explosion()");
        if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
            board.elements_to_damage_list.Remove(this);

        //Debug.Log(_x+","+_y + " = " + board.board_array_master[_x,_y,1]);
        if (board.board_array_master[_x, _y, 1] >= 0) //if there is something here
        {
            if (board.board_array_master[_x, _y, 1] < board.myRuleset.gem_length)//if this is a normal gem
            {
                if (board.board_array_master[_x, _y, 3] > 0)//if there is a restrain
                {
                    //print("......explode padlock hp:" + board.board_array_master[_x, _y, 3] + " status: " + board.board_array_master[_x, _y, 11]);
                    Debug.Log(" reset explosion " + _x + "," + _y);

                    if (board.board_array_master[_x, _y, 11] == 6)
                        Debug.LogError("6");
                    

                    board.board_array_master[_x, _y, 11] = 0;//reset explosion
                    board.reserved_for_primary_explosion.Remove(this);

                    board.board_array_master[_x, _y, 15]--;//restrain hp

                    if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                        {
                        board.number_of_elements_to_damage--;
                        board.number_of_padlocks_involved_in_explosion--;
                        }

                    //check restrain hp
                    if (board.board_array_master[_x, _y, 15] > 0)
                    {

                        if (board.board_array_master[_x, _y, 3] == 1)//padlock
                        {
                            SpriteRenderer sprite_restraint = my_padlock.GetComponent<SpriteRenderer>();
                            sprite_restraint.sprite = board.myTheme.lock_gem_hp[board.board_array_master[_x, _y, 15]-1];
                        }
                        else if (board.board_array_master[_x, _y, 3] == 2)//ice
                        {
                            SpriteRenderer sprite_restraint = my_ice.GetComponent<SpriteRenderer>();
                            sprite_restraint.sprite = board.myTheme.ice_hp[board.board_array_master[_x, _y, 15]-1];
                        }
                        else if (board.board_array_master[_x, _y, 3] == 3)//falling padlock
                        {
                            SpriteRenderer sprite_restraint = my_padlock.GetComponent<SpriteRenderer>();
                            sprite_restraint.sprite = board.myTheme.fallingPadlock_hp[board.board_array_master[_x, _y, 15] - 1];
                        }
                        else if (board.board_array_master[_x, _y, 3] == 4)//cage
                        {
                            SpriteRenderer sprite_restraint = my_padlock.GetComponent<SpriteRenderer>();
                            sprite_restraint.sprite = board.myTheme.cage_hp[board.board_array_master[_x, _y, 15] - 1];
                        }
                    }
                    else //the padlock hp is 0, so destroy the padlock/ice
                    {
                        if (board.board_array_master[_x, _y, 3] == 1)//padlock
                        {
                            board.garbageManager.GarbagePadlock(this);
                            board.immovable_elements.Remove(this);
                            board.board_array_master[_x, _y, 10] = 1;//now can fall

                            board.padlock_count--;
                            if ((board.myRuleset.win_requirement_selected == Ruleset.win_requirement.destroy_all_padlocks) && (board.padlock_count == 0))
                                board.Player_win();
                        }
                        else if (board.board_array_master[_x, _y, 3] == 2)//ice
                        {
                            board.garbageManager.GarbageIce(this);
                            board.immovable_elements.Remove(this);
                            board.board_array_master[_x, _y, 10] = 1;//now can fall

                            board.ice_count--;
                        }
                        else if (board.board_array_master[_x, _y, 3] == 3)//falling padlock
                        {
                            board.garbageManager.GarbageFallingPadlock(this);
                            board.immovable_elements.Remove(this);
   
                            board.padlock_count--;
                        }
                        else if (board.board_array_master[_x, _y, 3] == 4)//cage
                        {
                            board.garbageManager.GarbageCage(this);
                            board.immovable_elements.Remove(this);

                            board.padlock_count--;
                        }

                        board.board_array_master[_x, _y, 3] = 0;//restrain removed
                    }

                    If_all_explosion_are_completed();
                }
                else//no restrain
                {
                    board.activeCharacter.myCharacter.totalNumberOfGemsDestroyed[board.board_array_master[_x, _y, 1]]++;

                    if (board.myRuleset.give_bonus_select == Ruleset.give_bonus.advancedCharge)
                        board.AdvancedBonusGemCount(board.board_array_master[_x, _y, 1]);

                    Destroy_gem_avatar();
                }
            }
            else if ((board.board_array_master[_x, _y, 1] >= 41) && (board.board_array_master[_x, _y, 1] < 63)) //it is a block
            {
                Damage_block();
            }
            else if (board.board_array_master[_x, _y, 1] == 9)//this was a bonus
                Destroy_gem_avatar();
        }
        else//this tile is empty, so...
            {
            if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                board.number_of_elements_to_damage--;//strike off this explosion
            }
    }


    void Update_tile_hp()
    {
        if (board.board_array_master[_x, _y, 0] > 0)//if i can lose hp
        {
            board.board_array_master[_x, _y, 0]--;

            board.activeCharacter.myCharacter.score += board.globalRules.score_reward_for_damaging_tiles;

            board.uIManager.Update_score();

            //update tile sprite
            sprite_hp.sprite = board.myTheme.tile_hp[board.board_array_master[_x, _y, 0]];

            board.uIManager.Update_board_hp();

            if (board.myRuleset.tile_gift_hp > 0)
            {
                board.activeCharacter.myCharacter.currentHp += board.myRuleset.tile_gift_hp;

                board.uIManager.Update_hp();
            }

            if (board.myRuleset.tile_gift_time > 0)
                board.Add_time_bonus(board.myRuleset.tile_gift_time);


            if (board.myRuleset.tile_gift_moves > 0)
                board.Gain_turns(board.myRuleset.tile_gift_moves);
            

            if (board.HP_board <= 0)
            {
                if (board.myRuleset.win_requirement_selected == Ruleset.win_requirement.destroy_all_tiles)
                    board.Player_win();
                else if ((board.myRuleset.win_requirement_selected == Ruleset.win_requirement.take_all_tokens) && board.myRuleset.show_token_after_all_tiles_are_destroyed)
                    board.Show_all_token_on_board();
            }

        }
    }

    void Destroy_gem_avatar()
    {
        board.Add_time_bonus(board.myRuleset.time_bonus_for_gem_explosion);

        if (board.myRuleset.explosions_damages_tiles)
            Update_tile_hp();

        //update board counts:
        if (board.board_array_master[_x, _y, 4] == -200)
            board.number_of_token_on_board--;
        else if (board.board_array_master[_x, _y, 4] == -100)
            board.number_of_junk_on_board--;
        else if (board.board_array_master[_x, _y, 4] > 0)
            board.number_of_bonus_on_board--;

        //damage adjacent blocks
        if ((_y + 1 < board._Y_tiles) && (board.board_array_master[_x, _y + 1, 14] > 0))
        {
            board.number_of_elements_to_damage++;
            board.script_tiles_array[_x, _y + 1].Damage_block();

        }
        if ((_y - 1 >= 0) && (board.board_array_master[_x, _y - 1, 14] > 0))
        {
            board.number_of_elements_to_damage++;
            board.script_tiles_array[_x, _y - 1].Damage_block();

        }
        if ((_x + 1 < board._X_tiles) && (board.board_array_master[_x + 1, _y, 14] > 0))
        {
            board.number_of_elements_to_damage++;
            board.script_tiles_array[_x + 1, _y].Damage_block();

        }
        if ((_x - 1 >= 0) && (board.board_array_master[_x - 1, _y, 14] > 0))
        {
            board.number_of_elements_to_damage++;
            board.script_tiles_array[_x - 1, _y].Damage_block();

        }

        //explosion
        if (board.globalRules.show_score_of_this_move && board.score_of_this_turn_move > 0)
        {
            board.uIManager.the_gui_score_of_this_move.position = new Vector3(_x, -_y, -1f);
            if (explosion_score_script)
                explosion_score_script.Show_score(board.score_of_this_turn_move);
            board.score_of_this_turn_move = 0;
        }

        if (board.myTheme.gem_explosion_fx_rule_selected == ThemeTemplate.gem_explosion_fx_rule.for_each_gem)
        {
            if (board.board_array_master[_x, _y, 1] < board.myRuleset.gem_length)//if is a gem
                board.garbageManager.RecycleGemExplosionFX(_x, _y);

        }
        else if (board.myTheme.gem_explosion_fx_rule_selected == ThemeTemplate.gem_explosion_fx_rule.only_for_big_explosion)
        {
            if (use_fx_big_explosion_here > 0)
            {
                board.garbageManager.RecycleBigExplosionFX(_x, _y, use_fx_big_explosion_here);
                use_fx_big_explosion_here = 0;
            }
        }

        if (board.audioManager.play_this_bonus_sfx == -1)//you don't play sound explosion fx if you must play a bonus sfx
            board.audioManager.Play_sfx(board.audioManager.GetGemExplosionSfx(board.board_array_master[_x, _y, 1]));

        //print(_x + "," + _y + " = " + "DestroyAnimationStart");
        DestroyAnimationStart();

    }


    

    void Damage_block()
    {
        //Debug.Log("Damage_block");
        board.board_array_master[_x, _y, 14]--;

        if (board.board_array_master[_x, _y, 14] <= 0)//if the hit had destroy the block
        {
            board.garbageManager.Put_in_garbage(myContent);
            myContent = null;



            if ((board.board_array_master[_x, _y, 1] > 40) && (board.board_array_master[_x, _y, 1] < 50))//normal block
                board.immovable_elements.Remove(this);
            else if ((board.board_array_master[_x, _y, 1] > 60) && (board.board_array_master[_x, _y, 1] < 70))//generative block
                {
                board.GetGenerativeBlockInfo(new Vector2Int(_x, _y)).generatorIsOn = false;
                board.immovable_elements.Remove(this);
                }


            board.block_count--;
            //print("board.block_count: " + board.block_count);
            if ((board.myRuleset.win_requirement_selected == Ruleset.win_requirement.destroy_all_blocks) && (board.block_count == 0))
                board.Player_win();

            board.board_array_master[_x, _y, 14] = 0;
            //now this tile is empty
            //my_gem = null;
            board.board_array_master[_x, _y, 1] = -99;
            board.board_array_master[_x, _y, 13] = 0;

        }
        else //update block sprite
        {
            Debug.Log("update block sprite: " + board.board_array_master[_x, _y, 1]);

            if ((board.board_array_master[_x, _y, 1] > 40) && (board.board_array_master[_x, _y, 1] < 50)) //normal block
                myContent.mySpriteRenderer.sprite = board.myTheme.block_hp[board.board_array_master[_x, _y, 14] - 1];
            else if ((board.board_array_master[_x, _y, 1] > 50) && (board.board_array_master[_x, _y, 1] < 60)) //falling block
                myContent.mySpriteRenderer.sprite = board.myTheme.falling_block_hp[board.board_array_master[_x, _y, 14] - 1];
            else if ((board.board_array_master[_x, _y, 1] > 60) && (board.board_array_master[_x, _y, 1] < 70)) //generative block
            {
                Debug.Log("generative icon: " + (board.board_array_master[_x, _y, 14] - 1));
                myContent.mySpriteRenderer.sprite = board.myTheme.generative_block_hp[board.board_array_master[_x, _y, 14] - 1];
            }
        }

        if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
            board.number_of_elements_to_damage--;

        //Debug.Log(" reset explosion " + _x + "," + _y);
        if (board.board_array_master[_x, _y, 11] == 6)
            Debug.LogError("6");
        board.board_array_master[_x, _y, 11] = 0;//reset explosion
        If_all_explosion_are_completed();
    }


    public void Destroy_gem()
    {
        if (board.board_array_master[_x, _y, 1] < 9)//if this is a normal gem
            Update_gems_score();

        if (board.board_array_master[_x, _y, 4] > 0)
        {
            Debug.Log("DESTROY BONUS");
        }

        board.board_array_master[_x, _y, 1] = -99;//now this tile is empty (so it don't have color)
        board.board_array_master[_x, _y, 4] = 0; //no special
        board.board_array_master[_x, _y, 10] = 0; //can't fall

        if (board.board_array_master[_x, _y, 11] == 6)
            Debug.LogError("6");

        board.board_array_master[_x, _y, 11] = 0;//destruction is over

        board.garbageManager.Put_in_garbage(myContent);
        myContent = null;

        if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
            board.number_of_elements_to_damage--;

        If_all_explosion_are_completed();

    }

    void If_all_explosion_are_completed()
    {
        //print("If_all_explosion_are_completed() " + board.number_of_elements_to_damage + "..." + board.number_of_elements_to_damage_with_SwitchingGems);

        if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime || board.all_explosions_are_completed)
            return;

        //Debug.Log(_x + "," + _y + " switch: " + board.number_of_elements_to_damage_with_SwitchingGems  + " ...secondary: " + board.number_of_elements_to_damage + " ...bonus: " + board.number_of_elements_to_damage_with_bonus + " ... padlocks: " + board.number_of_padlocks_involved_in_explosion);
        if ((board.number_of_elements_to_damage + board.number_of_elements_to_damage_with_SwitchingGems + board.number_of_elements_to_damage_with_bonus) == 0 && board.number_of_padlocks_involved_in_explosion == 0)
        {
            //Debug.Log("all explosions are completed");
            board.all_explosions_are_completed = true;
            board.Start_update_board();
        }
    }

    

    void Update_gems_score()
    {
        if (board.board_array_master[_x, _y, 1] < 0)
            return;

        board.total_gems_on_board_at_start--;

        //fill bonus
        if (board.myRuleset.give_bonus_select == Ruleset.give_bonus.after_charge)
            board.uIManager.Update_bonus_fill(_x, _y, board.board_array_master[_x, _y, 1]);


        board.activeCharacter.myCharacter.totalNumberOfGemsDestroyed[board.board_array_master[_x, _y, 1]]++;
        board.activeCharacter.myCharacter.numberOfGemsCollect[board.board_array_master[_x, _y, 1]]++;



        if (board.player_turn)
        {

            if (board.player.myCharacter.numberOfGemsCollect[board.board_array_master[_x, _y, 1]]
                <= board.myRuleset.player.number_of_gems_to_destroy_to_win[board.board_array_master[_x, _y, 1]])
            {
                if (!board.player_win)
                {
                    board.player.myCharacter.totalNumberOfGemsRequiredColletted++;
                    board.player.myCharacter.totalNumberOfGemsRemaining--;

                    if (board.player.myCharacter.numberOfGemsCollect[board.board_array_master[_x, _y, 1]] == board.myRuleset.player.number_of_gems_to_destroy_to_win[board.board_array_master[_x, _y, 1]])
                        board.uIManager.This_gem_color_is_collected(board.board_array_master[_x, _y, 1]);
                }

                if (board.player.myCharacter.totalNumberOfGemsRemaining <= 0
                    && board.myRuleset.win_requirement_selected == Ruleset.win_requirement.collect_gems)
                {
                    board.uIManager.This_gem_color_is_collected(board.board_array_master[_x, _y, 1]);
                    board.Player_win();
                }
            }
            else
            {
                if (board.player.myCharacter.totalNumberOfGemsRemaining <= 0)
                    board.player.myCharacter.totalNumberOfExtraGemsCollettedAfterTheRequired++;
            }



        }
        else //is the enemy turn
        {


            if (board.enemy.myCharacter.numberOfGemsCollect[board.board_array_master[_x, _y, 1]]
                <= board.myRuleset.enemies[board.currentEnemy].number_of_gems_to_destroy_to_win[board.board_array_master[_x, _y, 1]])
            {
                board.enemy.myCharacter.totalNumberOfGemsRequiredColletted++;
                board.enemy.myCharacter.totalNumberOfGemsRemaining--;

                if (board.enemy.myCharacter.numberOfGemsCollect[board.board_array_master[_x, _y, 1]] == board.myRuleset.enemies[board.currentEnemy].number_of_gems_to_destroy_to_win[board.board_array_master[_x, _y, 1]])
                    board.uIManager.This_gem_color_is_collected(board.board_array_master[_x, _y, 1]);

                if (board.enemy.myCharacter.totalNumberOfGemsRemaining <= 0
                   && board.myRuleset.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
                    board.Player_lose();
            }

        }



        if (board.myRuleset.use_armor)
        {
            
            //damage the my oppoent according to his armor vulnerability 
            if (board.passiveCharacter.myCharacter.currentHp > 0 && board.myRuleset.gemExplosionOutcomes[board.board_array_master[_x, _y, 1]].damageOpponent > 0)
                {
                board.DamagePassiveCharacter(board.board_array_master[_x, _y, 1], board.myRuleset.gemExplosionOutcomes[board.board_array_master[_x, _y, 1]].damageOpponent);
                }

            //damage himself according to his armor vulnerability 
            if (board.activeCharacter.myCharacter.currentHp > 0 && board.myRuleset.gemExplosionOutcomes[board.board_array_master[_x, _y, 1]].damageMe > 0)
                {
                board.DamageActiveCharacter(board.board_array_master[_x, _y, 1], board.myRuleset.gemExplosionOutcomes[board.board_array_master[_x, _y, 1]].damageMe);
                }

            //heal the current turn owner = the player
            if (board.myRuleset.gemExplosionOutcomes[board.board_array_master[_x, _y, 1]].healMe > 0)
                {
                board.Heal_me(board.myRuleset.gemExplosionOutcomes[board.board_array_master[_x, _y, 1]].healMe);
                }

                board.uIManager.Update_hp();
        }

        
    }
    #endregion



    void Generate_a_new_gem_in_this_tile()
    {
        //create a new gem in this tile
        board.count_gems_created++;
        random_color = (UnityEngine.Random.Range(0, board.myRuleset.gem_length));

        //Don't create 3 gem with the same color subsequently
        if (two_last_colors_created[0] == two_last_colors_created[1])
        {
            if (random_color == two_last_colors_created[0])
            {
                //update the color choiced				
                if (random_color + 1 < board.myRuleset.gem_length)
                    random_color++;
                else
                    random_color = 0;
            }
        }

        //avoid 3 gem with the same color on x axis too
        if ((_x >= 2) && (board.board_array_master[_x - 1, _y, 12] == 1) && (board.board_array_master[_x - 2, _y, 12] == 1))
        {   //if this gem not will fall	
            if (!((_y < board._Y_tiles - 1) && (board.board_array_master[_x, _y + 1, 1] >= 0))) //there are no empty tile under me
            {
                if (random_color == board.board_array_master[_x - 1, _y, 1])
                {
                    //update the color choiced
                    if (random_color + 1 < board.myRuleset.gem_length)
                        random_color++;
                    else
                        random_color = 0;

                }

            }
        }

        board.board_array_master[_x, _y, 1] = random_color;
        //update list of color choiced by this leader-tile
        two_last_colors_created[0] = two_last_colors_created[1];
        two_last_colors_created[1] = random_color;

        board.board_array_master[_x, _y, 10] = 1;//now this tile have a gem 

        //create gem avatar
        myContent = board.garbageManager.Take_from_garbage(_x,_y, TypeOfContent.Gem);

    }


    public void Fall_by_one_step(int fall_direction)//0 down, 1 down R, 2 down L
    {
        if (board.board_array_master[_x, _y, 11] == 6)
            Debug.LogError("6");

        if (fall_direction == 0 && board.board_array_master[_x, _y + 1, 1] == 6)
            Debug.LogError(" 6 under me!");

        if (fall_direction == 1 && board.ThereIsATileHere(_x + 1, _y + 1)) 
        {
            if (board.board_array_master[_x + 1, _y + 1, 1] == 6)
            Debug.LogError(" 6 under me! B");
        }

        if (fall_direction == 2 && board.ThereIsATileHere(_x - 1, _y + 1)) 
        {
            if (board.board_array_master[_x - 1, _y + 1, 1] == 6)
            Debug.LogError(" 6 under me! C");
        }


        if (fall_direction == 0)
        {
            //move the gem on the tile next up the last tile scanned
            board.board_array_master[_x, _y + 1, 1] = board.board_array_master[_x, _y, 1];
            //color from old tile position
            board.board_array_master[_x, _y, 1] = -99;

            //special
            board.board_array_master[_x, _y + 1, 4] = board.board_array_master[_x, _y, 4];

            //update falling block HP
            board.board_array_master[_x, _y + 1, 14] = board.board_array_master[_x, _y, 14];


            board.board_array_master[_x, _y, 4] = 0;

            //the gem go in the new tile position
            board.board_array_master[_x, _y + 1, 10] = 1;
            //empty the old tile position
            board.board_array_master[_x, _y, 10] = 0;

            //show the falling animation
            board.script_tiles_array[_x, _y + 1].myContent = myContent;
            board.script_tiles_array[_x, _y + 1].myContent.GetComponent<TileContent>().TrackMyOwner(board.script_tiles_array[_x, _y + 1]); //DEBUG
            myContent = null;

            board.board_array_master[_x, _y + 1, 11] = board.board_array_master[_x, _y, 11];
            board.board_array_master[_x, _y, 11] = 0;

            //move restrain
            if (board.board_array_master[_x, _y, 3] == 3)//is falling padlock
            {
                //pass values to next gem
                board.board_array_master[_x, _y + 1, 15] = board.board_array_master[_x, _y, 15];
                board.board_array_master[_x, _y + 1, 3] = board.board_array_master[_x, _y, 3];
                board.script_tiles_array[_x, _y + 1].my_padlock = board.script_tiles_array[_x, _y].my_padlock;

                //remove values from previous gem
                board.board_array_master[_x, _y, 15] = 0;
                board.board_array_master[_x, _y, 3] = 0;
                board.script_tiles_array[_x, _y].my_padlock = null;
            }

            board.script_tiles_array[_x, _y + 1].Update_gem_position();

        }
        else if (fall_direction == 1)
        {
            //move the gem on the tile next up the last tile scanned
            board.board_array_master[_x + 1, _y + 1, 1] = board.board_array_master[_x, _y, 1];
            //color from old tile position
            board.board_array_master[_x, _y, 1] = -99;

            //special
            board.board_array_master[_x + 1, _y + 1, 4] = board.board_array_master[_x, _y, 4];
            board.board_array_master[_x, _y, 4] = 0;

            //the gem go in the new tile position
            board.board_array_master[_x + 1, _y + 1, 10] = 1;
            //empty the old tile position
            board.board_array_master[_x, _y, 10] = 0;

            //show the falling animation
            board.script_tiles_array[_x + 1, _y + 1].myContent = myContent;
            board.script_tiles_array[_x + 1, _y + 1].myContent.GetComponent<TileContent>().TrackMyOwner(board.script_tiles_array[_x, _y + 1]); //DEBUG
            myContent = null;

            

            board.board_array_master[_x + 1, _y + 1, 11] = board.board_array_master[_x, _y, 11];
            board.board_array_master[_x, _y, 11] = 0;

            //move restraint
            if (board.board_array_master[_x, _y, 3] == 3)//is falling padlock
            {
                board.board_array_master[_x + 1, _y + 1, 15] = board.board_array_master[_x, _y, 15];
                board.board_array_master[_x + 1, _y + 1, 3] = board.board_array_master[_x, _y, 3];
                board.script_tiles_array[_x + 1, _y + 1].my_padlock = board.script_tiles_array[_x, _y].my_padlock;

                //remove values from previous gem
                board.board_array_master[_x, _y, 15] = 0;
                board.board_array_master[_x, _y, 3] = 0;
                board.script_tiles_array[_x, _y].my_padlock = null;
            }

            board.script_tiles_array[_x + 1, _y + 1].Update_gem_position();
        }
        else if (fall_direction == 2)
        {
            //move the gem on the tile next up the last tile scanned
            board.board_array_master[_x - 1, _y + 1, 1] = board.board_array_master[_x, _y, 1];
            //color from old tile position
            board.board_array_master[_x, _y, 1] = -99;

            //special
            board.board_array_master[_x - 1, _y + 1, 4] = board.board_array_master[_x, _y, 4];
            board.board_array_master[_x, _y, 4] = 0;

            //the gem go in the new tile position
            board.board_array_master[_x - 1, _y + 1, 10] = 1;
            //empty the old tile position
            board.board_array_master[_x, _y, 10] = 0;

            //show the falling animation
            board.script_tiles_array[_x - 1, _y + 1].myContent = myContent;
            board.script_tiles_array[_x - 1, _y + 1].myContent.GetComponent<TileContent>().TrackMyOwner(board.script_tiles_array[_x, _y + 1]); //DEBUG
            myContent = null;

            board.board_array_master[_x - 1, _y + 1, 11] = board.board_array_master[_x, _y, 11];
            board.board_array_master[_x, _y, 11] = 0;

            //move restrain
            if (board.board_array_master[_x, _y, 3] == 3)//is falling padlock
            {
                board.board_array_master[_x - 1, _y + 1, 15] = board.board_array_master[_x, _y, 15];
                board.board_array_master[_x - 1, _y + 1, 3] = board.board_array_master[_x, _y, 3];
                board.script_tiles_array[_x - 1, _y + 1].my_padlock = board.script_tiles_array[_x, _y].my_padlock;

                //remove values from previous gem
                board.board_array_master[_x, _y, 15] = 0;
                board.board_array_master[_x, _y, 3] = 0;
                board.script_tiles_array[_x, _y].my_padlock = null;
            }

            board.script_tiles_array[_x - 1, _y + 1].Update_gem_position();

        }

        //board.number_of_gems_to_move--;

        //Debug.Log("Fall_by_one_step END" + _x + "," + _y + " = " + board.board_array_master[_x, _y, 11]);
    }

    void Decide_what_create()
    {
        if (board.Emit_special_element())
        {
            
            int element_id = board.Random_choose_special_element_to_create();

            if (element_id == 0)//no special availble now
                Generate_a_new_gem_in_this_tile();//so generate a simple gem
            else
            {
                myContent = board.garbageManager.Take_from_garbage(_x, _y, TypeOfContent.None);//TypeOfContent.None becasue it will set in  Create_special_element(element_id);
                Create_special_element(element_id);
            }

        }
        else
            Generate_a_new_gem_in_this_tile();
    }



    void Search_last_empty_tile_under_me()
    {
        for (int yy = 1; (_y + yy) <= board._Y_tiles - 1; yy++)//scan tiles under me
        {
            if ((board.board_array_master[_x, (_y + yy), 0] == -1) //if I find no tile
                || (board.board_array_master[_x, _y + yy, 1] >= 0)) //or a tile with something
            {

                //move the gem on the tile next up the last tile scanned
                board.board_array_master[_x, _y + yy - 1, 1] = board.board_array_master[_x, _y, 1];
                //color from old tile position
                board.board_array_master[_x, _y, 1] = -99;

                //special
                board.board_array_master[_x, _y + yy - 1, 4] = board.board_array_master[_x, _y, 4];
                board.board_array_master[_x, _y, 4] = 0;

                //the gem go in the new tile position
                board.board_array_master[_x, _y + yy - 1, 10] = 1;
                //empty the old tile position
                board.board_array_master[_x, _y, 10] = 0;

                //show the falling animation
                board.script_tiles_array[_x, (_y + yy - 1)].myContent = myContent;
                board.script_tiles_array[_x, (_y + yy - 1)].Update_gem_position();
                myContent = null;

                break;
            }
            else if ((_y + yy) == board._Y_tiles - 1) //if I'm on the last row
            {

                //move gem color on last tile
                board.board_array_master[_x, _y + yy, 1] = board.board_array_master[_x, _y, 1];
                //remove color from old tile
                board.board_array_master[_x, _y, 1] = -99;

                //special 
                board.board_array_master[_x, _y + yy, 4] = board.board_array_master[_x, _y, 4];
                board.board_array_master[_x, _y, 4] = 0;

                //gem go in the new position
                board.board_array_master[_x, _y + yy, 10] = 1;
                //empty the old tile
                board.board_array_master[_x, _y, 10] = 0;


                //show falling animation
                board.script_tiles_array[_x, (_y + yy)].myContent = myContent;
                myContent = null;
                //board.script_tiles_array[_x, (_y + yy)].myContent.transform.parent = board.script_tiles_array[_x, (_y + yy)].transform;
                board.script_tiles_array[_x, (_y + yy)].Update_gem_position();


                break;
            }

        }

    }




}
