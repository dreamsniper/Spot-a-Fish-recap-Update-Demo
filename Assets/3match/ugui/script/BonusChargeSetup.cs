using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusChargeSetup : MonoBehaviour {

    public Bonus[] playerBonuses = new Bonus[7];



    public void ChargeSetup()
    {
        Board_C myBoard = GetComponent<Board_C>();

        for (int i = 0; i < myBoard.player.myCharacter.bonus_slot.Length; i++)
            myBoard.player.myCharacter.bonus_slot[i] = playerBonuses[i];

        for (int i = 0; i < myBoard.player.myUI.gui_bonus_bar.transform.childCount; i++)
            Destroy(myBoard.player.myUI.gui_bonus_bar.transform.GetChild(i).gameObject);

        myBoard.uIManager.Initiate_ugui();


    }
}
