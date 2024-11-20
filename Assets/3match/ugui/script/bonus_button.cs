using UnityEngine.UI;
using UnityEngine;
using System.Collections;


    public class bonus_button : MonoBehaviour {

    [HideInInspector]
	public bool player;
    [HideInInspector]
    public int slot_number;

	public Image full_image;
        [HideInInspector]
        public Color base_color;
        [HideInInspector]
        public float currentFill;

    public Button my_button;
    //public Toggle my_toggle;
    [HideInInspector]
    public int cost;
    [HideInInspector]
    public Bonus myBonus; 
    [HideInInspector]
    public float bonusStrength; //for heal, damage and timer
    [HideInInspector]
    public Board_C board;

    enum UpdateTurn
    {
        DoNothing,
        EnemyMoveAgain,
        MoveToNextTurn
    }
    UpdateTurn wathToDoAfterFillUpdate = UpdateTurn.DoNothing;



    public virtual void Update_fill()
	{
		if (player)
			{
			full_image.fillAmount = Mathf.Lerp(0,1, currentFill / (float)cost);

			if (full_image.fillAmount == 1)
                {
                board.player.myCharacter.bonus_ready[slot_number] = true;
                my_button.interactable = true;
                }
            else
                {
                board.player.myCharacter.bonus_ready[slot_number] = false;
                my_button.interactable = false;
                }
    }
		else
			{
			full_image.fillAmount = Mathf.Lerp(0,1, currentFill / (float)cost);

            if (full_image.fillAmount == 1)
                board.enemy.myCharacter.bonus_ready[slot_number] = true;
            else
                board.enemy.myCharacter.bonus_ready[slot_number] = false;

            }

        if (wathToDoAfterFillUpdate == UpdateTurn.EnemyMoveAgain)//because heal or damage don't cost a turn
        {
            wathToDoAfterFillUpdate = UpdateTurn.DoNothing;

            board.player_turn = false;
            board.player_can_move = false;

            board.Invoke("Enemy_play", board.globalRules.enemy_move_delay);
        }
        else if (wathToDoAfterFillUpdate == UpdateTurn.MoveToNextTurn)
        {
            wathToDoAfterFillUpdate = UpdateTurn.DoNothing;
            board.Update_turn_order_after_a_bad_move();
        }
    }

    public void Click_me()
	{
		if (board.player_can_move || board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
			{
			if (board.bonus_select == Bonus.None) 
				Activate();
			else
				{
				if (board.uIManager.slot_bonus_ico_selected == slot_number) //if you re-click on me, then deselect me
					Deselect();
				else //deselect previous slot, and activate this
					{
                    board.uIManager.playerUI.gui_bonus_ico_array[board.uIManager.slot_bonus_ico_selected].GetComponent<bonus_button>().Deselect();
					Activate();
					}
				}
			}
		else
			{
			if (board.player.myCharacter.bonus_slot[slot_number] == Bonus.GiveMoreTime)//you can use time bonus even when gems are falling
				Activate();
			}
	}

    public void Deselect()
    {
        if (player)
        {
            board.uIManager.slot_bonus_ico_selected = -1;
            full_image.color = base_color;
        }
        else
        {
            full_image.color = base_color;
        }

        if (board.current_moveStatus == Board_C.moveStatus.waitingNewMove)
            {
            board.main_gem_selected_x = -10;
            board.main_gem_selected_y = -10;
            board.main_gem_color = -10;
            board.minor_gem_destination_to_x = -10;
            board.minor_gem_destination_to_y = -10;
            board.minor_gem_color = -10;
            }


        if (board.bonus_select == Bonus.HealMe)
        {
            board.bonus_select = Bonus.None;

            if (board.globalRules.use_heal_cost_a_turn)
                {
                wathToDoAfterFillUpdate = UpdateTurn.MoveToNextTurn;
                return;
                }
            else
                {
                if (!player)
                    {
                    wathToDoAfterFillUpdate = UpdateTurn.EnemyMoveAgain;
                    return;
                    }
                }
        }
        else if (board.bonus_select == Bonus.DamageOpponent)
        {
            board.bonus_select = Bonus.None;

            if (board.globalRules.use_damage_cost_a_turn)
            {
                wathToDoAfterFillUpdate = UpdateTurn.MoveToNextTurn;
                return;
            }
            else
            {
                if (!player)
                {
                    wathToDoAfterFillUpdate = UpdateTurn.EnemyMoveAgain;
                    return;
                }
            }
        }
        

        board.bonus_select = Bonus.None;


    }

    public void EnemyActivate()
    {
        board.uIManager.slot_bonus_ico_selected = slot_number;
        full_image.color = Color.white;

        Invoke("Activate", board.globalRules.enemy_move_delay);
    }


    public virtual void Activate()
	{

        if (board.bonus_select == Bonus.GiveMoreTime)
			{
			board.Add_time_bonus(bonusStrength);
			Reset_fill();
			board.audioManager.Play_bonus_sfx(8,true);
			}
		else if (board.bonus_select == Bonus.GiveMoreMoves)
			{
			board.Gain_turns((int)bonusStrength);
			Reset_fill();
			board.audioManager.Play_bonus_sfx(9,true);
			}
		else if (board.bonus_select == Bonus.HealMe)
			{
			board.Heal_me((int)bonusStrength);
			Reset_fill();
			board.audioManager.Play_bonus_sfx(10,true);
			}
        else if (board.bonus_select == Bonus.DamageOpponent)
            {
            board.Damage_opponent((int)bonusStrength);
            Reset_fill();
            board.audioManager.Play_bonus_sfx(11, true);
            }
       else
		    {
            board.uIManager.slot_bonus_ico_selected = slot_number;
			full_image.color = Color.white;
		    }
	}

	public virtual void Reset_fill()
	{
        if (full_image.fillAmount < 1)
        {
            Debug.LogError("Reset fill: " + full_image.fillAmount);
            return;
        }

        Deselect();

        /*
		if (player)
			{
			board.filling_player_bonus[slot_number] = 0;
			board.bonus_ready[slot_number] = false;
			}
		else
			{
			board.filling_enemy_bonus[slot_number] = 0;
			board.enemy_bonus_ready[slot_number] = false;
			}

		Update_fill();*/
	}
	

}
