using UnityEngine;
using UnityEngine.UI;
using System.Collections;


    public class inventory_bonus_button : MonoBehaviour {


	public int my_id;
	public bonus_inventory my_inventory;
	Image my_image;
	Color base_color;


	void Start()
		{
		my_image = GetComponent<Image>();
		base_color = my_image.color;
		}

	public void Click_me()
		{
		if (my_inventory.board.player_can_move || my_inventory.board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
			{
			if (my_inventory.board.bonus_select == Bonus.None) 
				Activate();
			else
				{
				if (my_inventory.board.bonus_select == (Bonus)my_id) //if you re-click on me, then deselect me
					Deselect();
				else //deselect previous slot, and activate this
					{
					my_inventory.bonus_list[(int)my_inventory.board.bonus_select].GetComponent<inventory_bonus_button>().Deselect();
					Activate();
					}
				}
			}
		}

    public void EnemyActivate()
    {
        my_image.color = Color.magenta;

        Invoke("Activate", my_inventory.board.globalRules.enemy_move_delay);
    }

    public void Activate()
		{

		my_inventory.board.bonus_select = (Bonus)my_id;

		if (my_inventory.board.bonus_select == Bonus.GiveMoreTime)
			{
			my_inventory.board.Add_time_bonus(my_inventory.board.globalRules.add_time_bonus);
			my_inventory.board.player.myCharacter.bonus_inventory[my_id]--;
			my_inventory.Update_bonus_count(my_id);
			my_inventory.board.bonus_select = Bonus.None;
			my_inventory.board.audioManager.Play_bonus_sfx(8,true);
			}
		else if (my_inventory.board.bonus_select == Bonus.GiveMoreMoves)
			{
			my_inventory.board.Gain_turns(my_inventory.board.globalRules.add_moves_bonus);

			my_inventory.board.player.myCharacter.bonus_inventory[my_id]--;
			my_inventory.Update_bonus_count(my_id);
            //my_inventory.board.bonus_select = Board_C.bonus.none;
            Deselect();

            my_inventory.board.audioManager.Play_bonus_sfx(9,true);
			}
		else if (my_inventory.board.bonus_select == Bonus.HealMe)
			{
			if (my_inventory.player)
				{
				if ((my_inventory.board.player.myCharacter.currentHp + my_inventory.board.player.myCharacter.heal_me_hp_bonus) <= my_inventory.board.player.myCharacter.maxHp)
					my_inventory.board.player.myCharacter.currentHp += my_inventory.board.player.myCharacter.heal_me_hp_bonus;
				else
					my_inventory.board.player.myCharacter.currentHp = my_inventory.board.player.myCharacter.maxHp;

				my_inventory.board.player.myCharacter.bonus_inventory[my_id]--;
				}
			else
				{
				if ((my_inventory.board.enemy.myCharacter.currentHp + my_inventory.board.enemy.myCharacter.heal_me_hp_bonus) <= my_inventory.board.enemy.myCharacter.maxHp)
					my_inventory.board.enemy.myCharacter.currentHp += my_inventory.board.enemy.myCharacter.heal_me_hp_bonus;
				else
					my_inventory.board.enemy.myCharacter.currentHp = my_inventory.board.enemy.myCharacter.maxHp;

				my_inventory.board.enemy.myCharacter.bonus_inventory[my_id]--;
				}
			
			my_inventory.board.uIManager.Update_hp();
			
			my_inventory.Update_bonus_count(my_id);
            //my_inventory.board.bonus_select = Board_C.bonus.none;
            Deselect();

            if (my_inventory.board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased && my_inventory.board.globalRules.use_heal_cost_a_turn)
			    my_inventory.board.Check_secondary_explosions();
            else if (!my_inventory.player)
                my_inventory.board.Enemy_play();

            my_inventory.board.audioManager.Play_bonus_sfx(10,true);
			}
        else if (my_inventory.board.bonus_select == Bonus.DamageOpponent)
        {
            if (my_inventory.player)
            {
                if ((my_inventory.board.enemy.myCharacter.currentHp - my_inventory.board.player.myCharacter.damage_opponent_bonus) <= 0)
                    my_inventory.board.enemy.myCharacter.currentHp = 0;
                else
                    my_inventory.board.enemy.myCharacter.currentHp -= my_inventory.board.player.myCharacter.damage_opponent_bonus;

                    my_inventory.board.player.myCharacter.bonus_inventory[my_id]--;
            }
            else
            {
                if ((my_inventory.board.player.myCharacter.currentHp - my_inventory.board.enemy.myCharacter.damage_opponent_bonus) <= 0)
                    my_inventory.board.player.myCharacter.currentHp = 0;
                else
                    my_inventory.board.player.myCharacter.currentHp -= my_inventory.board.enemy.myCharacter.damage_opponent_bonus;

                my_inventory.board.enemy.myCharacter.bonus_inventory[my_id]--;
            }

            my_inventory.board.uIManager.Update_hp();

            my_inventory.Update_bonus_count(my_id);
            //my_inventory.board.bonus_select = Board_C.bonus.none;
            Deselect();

            if (my_inventory.board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased && my_inventory.board.globalRules.use_damage_cost_a_turn)
                my_inventory.board.Check_secondary_explosions();
            else if (!my_inventory.player)
                my_inventory.board.Enemy_play();

            my_inventory.board.audioManager.Play_bonus_sfx(11, true);

        }
        else
			{
			my_image.color = Color.white;
			}
		}

	public void Deselect()
		{
        print("deselec");
		my_inventory.board.bonus_select = Bonus.None;
		

		my_image.color = base_color;

		if (my_inventory.board.current_moveStatus == Board_C.moveStatus.waitingNewMove)
            {
            my_inventory.board.main_gem_selected_x = -10;
		    my_inventory.board.main_gem_selected_y = -10;
		    my_inventory.board.main_gem_color = -10;
		    my_inventory.board.minor_gem_destination_to_x = -10;
		    my_inventory.board.minor_gem_destination_to_y = -10;
		    my_inventory.board.minor_gem_color = -10;
            }
    }
}

