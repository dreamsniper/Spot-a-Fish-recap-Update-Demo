using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedBonusButton : bonus_button{

    public Text myName;
    public Image icon;
    [HideInInspector]
    public bool isReady;

    [Space()]
    public GameObject AndContainer;
    public GameObject[] and_gems;
    public Text[] and_gemsCount;


    [Space()]
    public GameObject OrContainer;
    public Text targetNumber;
    public GameObject[] or_gems;
    int authorizedGems;


    AdvancedChargeBonus thisBonus;

    //or
    [HideInInspector]
    public int currentOrCount;
    //and
    public int[] currentAndCount;


    public void StartMe(AdvancedChargeBonus _thisBonus,  int _slot_number, bool _player, Board_C _board) {

        slot_number = _slot_number;
        player = _player;
        board = _board;
        thisBonus = _thisBonus;

        myName.text = thisBonus.name;
        icon.sprite = thisBonus.icon;
        isReady = false;
        my_button.interactable = false;
        cost = 0;
        myBonus = _thisBonus.myBonus;
        bonusStrength = _thisBonus.strength;

        currentOrCount = 0;
        authorizedGems = 0;
        currentAndCount = new int[thisBonus.targetCostByGemColor.Length];

        if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
        {
            OrContainer.SetActive(false);

            full_image = AndContainer.GetComponent<Image>();
            base_color = full_image.color;

            for (int i = 0; i < thisBonus.targetCostByGemColor.Length; i++)
            {
                if (thisBonus.targetCostByGemColor[i] > 0)
                {
                    and_gems[i].GetComponent<Image>().sprite = board.myTheme.gem_colors[i];
                    and_gems[i].SetActive(true);
                    and_gemsCount[i].text = "0/" + thisBonus.targetCostByGemColor[i].ToString();
                    cost += thisBonus.targetCostByGemColor[i];
                }
                else
                    and_gems[i].SetActive(false);
            }

            AndContainer.SetActive(true);

        }
        else if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
        {
            AndContainer.SetActive(false);

            full_image = OrContainer.GetComponent<Image>();
            base_color = full_image.color;

            for (int i = 0; i < thisBonus.allowedGemColors.Length; i++)
            {
                if (thisBonus.allowedGemColors[i])
                    {
                    authorizedGems++;
                    or_gems[i].GetComponent<Image>().sprite = board.myTheme.gem_colors[i];
                    or_gems[i].SetActive(true);
                    }
                else
                    or_gems[i].SetActive(false);
            }

            targetNumber.text = "0/" + thisBonus.targetTotal.ToString();

            cost = thisBonus.targetTotal;

            OrContainer.SetActive(true);

        }

        Update_fill();

    }

    public AdvancedChargeBonus.AdvancedChargeBonusCostRule GetMyRule()
    {
        return thisBonus.AdvancedChargeBonus_costRule;
    }

    public override void Reset_fill()
    {
        base.Reset_fill();

        if (player)
        {
            if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
            {
                for (int i = 0; i < thisBonus.targetCostByGemColor.Length; i++)
                    currentAndCount[i] = 0;

                if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                {
                    for (int i = 0; i < thisBonus.targetCostByGemColor.Length; i++)
                    {
                        if (thisBonus.targetCostByGemColor[i] > 0)
                            board.player.myCharacter.gemColorAdvancedChargeBonusPool[i] -= thisBonus.targetCostByGemColor[i];
                    }
      
                }
            }
            else if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
            {
                currentOrCount = 0;

                int removeThis = thisBonus.targetTotal;

                while (removeThis > 0)
                {
                    for (int i = 0; i < thisBonus.allowedGemColors.Length; i++)
                    {
                        if (thisBonus.allowedGemColors[i])
                        {
                            if (board.player.myCharacter.gemColorAdvancedChargeBonusPool[i] > 0)
                            {
                                board.player.myCharacter.gemColorAdvancedChargeBonusPool[i]--;
                                removeThis--;

                                if (removeThis <= 0)
                                    break;
                            }
                        }
                    }
                }
            }

            if (board.player.myUI.gemPoolCount)
                board.player.myUI.gemPoolCount.UpdateMe();
        }
        else //enemy
        {
            if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
            {
                for (int i = 0; i < thisBonus.targetCostByGemColor.Length; i++)
                    currentAndCount[i] = 0;

                if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                {
                    for (int i = 0; i < thisBonus.targetCostByGemColor.Length; i++)
                    {
                        if (thisBonus.targetCostByGemColor[i] > 0)
                            board.enemy.myCharacter.gemColorAdvancedChargeBonusPool[i] -= thisBonus.targetCostByGemColor[i];
                    }

                }
            }
            else if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
            {
                currentOrCount = 0;

                int removeThis = thisBonus.targetTotal;

                while (removeThis > 0)
                {
                    for (int i = 0; i < thisBonus.allowedGemColors.Length; i++)
                    {
                        if (thisBonus.allowedGemColors[i])
                        {
                            if (board.enemy.myCharacter.gemColorAdvancedChargeBonusPool[i] > 0)
                            {
                                board.enemy.myCharacter.gemColorAdvancedChargeBonusPool[i]--;
                                removeThis--;

                                if (removeThis <= 0)
                                    break;
                            }
                        }
                    }
                }

            }

            if (board.enemy.myUI.gemPoolCount)
                board.enemy.myUI.gemPoolCount.UpdateMe();
        }

        if (board.myRuleset.allTheBonusesShareTheSameGemPool)
            board.uIManager.UpdateAllAdvancedChargeBonusUGUI();
        else
            UpdateGUI();
    }

    public void UpdateGUI()
    {
        bool isReadyTemp = true;
        currentFill = 0;

        if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
        {
            

            for (int i = 0; i < thisBonus.targetCostByGemColor.Length; i++)
            {
                if (thisBonus.targetCostByGemColor[i] <= 0)
                    continue;

                if (board.myRuleset.allTheBonusesShareTheSameGemPool)
                {
                    if (board.player_turn)
                        currentAndCount[i] = board.player.myCharacter.gemColorAdvancedChargeBonusPool[i];
                    else//enemy
                        currentAndCount[i] = board.enemy.myCharacter.gemColorAdvancedChargeBonusPool[i];
                }

                int textCount = currentAndCount[i];

                if (textCount >= thisBonus.targetCostByGemColor[i])
                    textCount = thisBonus.targetCostByGemColor[i];
                else
                    isReadyTemp = false;

                and_gemsCount[i].text = textCount.ToString() + "/" + thisBonus.targetCostByGemColor[i].ToString();

                currentFill += textCount;
            }


        }
        else if (thisBonus.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
        {
            int textCount = currentOrCount;

            if (textCount >= thisBonus.targetTotal)
                textCount = thisBonus.targetTotal;
            else
                isReadyTemp = false;


            targetNumber.text = textCount.ToString() + "/" + thisBonus.targetTotal.ToString();
            currentFill = textCount;

        }


        isReady = isReadyTemp;

        Update_fill();

        if (player)
            my_button.interactable = isReady;
    }

    
    public override void Activate()
    {
        //if (board.player_turn)
            board.bonus_select = thisBonus.myBonus;


        base.Activate();
    }
}
