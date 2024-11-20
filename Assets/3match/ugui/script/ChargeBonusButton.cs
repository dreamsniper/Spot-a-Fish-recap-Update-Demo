using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBonusButton : bonus_button
{
    void Start()
    {
        my_button.interactable = false;

        full_image.sprite = GetComponent<Image>().sprite;
        base_color = full_image.color;

        board = Board_C.this_board.GetComponent<Board_C>();

        if (player)
            {
            cost = board.player.myCharacter.charge_bonus_cost[slot_number];

            if (myBonus == Bonus.HealMe)
                bonusStrength = board.player.myCharacter.heal_me_hp_bonus;
            else if (myBonus == Bonus.DamageOpponent)
                bonusStrength = board.player.myCharacter.damage_opponent_bonus;
            else if (myBonus == Bonus.GiveMoreTime)
                bonusStrength = board.globalRules.add_time_bonus;
            else if (myBonus == Bonus.GiveMoreMoves)
                bonusStrength = board.globalRules.add_moves_bonus;
        }
        else
            {
            cost = board.enemy.myCharacter.charge_bonus_cost[slot_number];

            if (myBonus == Bonus.HealMe)
                bonusStrength = board.enemy.myCharacter.heal_me_hp_bonus;
            else if (myBonus == Bonus.DamageOpponent)
                bonusStrength = board.enemy.myCharacter.damage_opponent_bonus;
            }

        

        //for some reason, if I don't force unity to do this, unity don't show the partial sprite until it is completed
        full_image.gameObject.SetActive(false);
        full_image.gameObject.SetActive(true);

        Update_fill();

    }


    public override void Activate()
    {
        if (board.player_turn)
            board.bonus_select = board.player.myCharacter.bonus_slot[slot_number];


        base.Activate();
    }

    public override void Update_fill()
    {

        if (player)
            currentFill = (float)board.player.myCharacter.filling_bonus[slot_number];
        else
            currentFill = (float)board.enemy.myCharacter.filling_bonus[slot_number];

        base.Update_fill();

    }

    public override void Reset_fill()
    {
        Deselect();

        if (player)
        {
            board.player.myCharacter.filling_bonus[slot_number] = 0;
            board.player.myCharacter.bonus_ready[slot_number] = false;
        }
        else
        {
            board.enemy.myCharacter.filling_bonus[slot_number] = 0;
            board.enemy.myCharacter.bonus_ready[slot_number] = false;
        }

        Update_fill();
    }
}
