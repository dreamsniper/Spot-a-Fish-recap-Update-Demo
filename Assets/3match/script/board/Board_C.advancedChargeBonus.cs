using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Board_C : MonoBehaviour
{

    public void AdvancedBonusGemCount(int gemColor)
    {
        if (myRuleset.allTheBonusesShareTheSameGemPool)
            {
            if (activeCharacter.myUI.gemPoolCount == null)
                return;

            activeCharacter.myCharacter.gemColorAdvancedChargeBonusPool[gemColor]++;
            activeCharacter.myUI.gemPoolCount.UpdateMe();
            }


        for (int i = 0; i < activeCharacter.myUI.advancedBonusButton.Length; i++)
        {
            if (activeCharacter.myCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
            {
                for (int c = 0; c < activeCharacter.myCharacter.advancedChargeBonuses[i].allowedGemColors.Length; c++)
                {
                    if (c != gemColor)
                        continue;

                    if (activeCharacter.myCharacter.advancedChargeBonuses[i].allowedGemColors[c])
                    {
                        activeCharacter.myUI.advancedBonusButton[i].currentOrCount++;
                        activeCharacter.myUI.advancedBonusButton[i].UpdateGUI();
                    }
                }
            }
            else if (activeCharacter.myCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
            {
                for (int c = 0; c < activeCharacter.myCharacter.advancedChargeBonuses[i].targetCostByGemColor.Length; c++)
                {
                    if (c != gemColor)
                        continue;

                    if (activeCharacter.myCharacter.advancedChargeBonuses[i].targetCostByGemColor[c] > 0)
                    {
                        activeCharacter.myUI.advancedBonusButton[i].currentAndCount[c]++;
                        activeCharacter.myUI.advancedBonusButton[i].UpdateGUI();
                    }
                }

            }
        }

    }
}
