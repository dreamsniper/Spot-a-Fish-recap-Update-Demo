using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "3Match/AdvancedChargeBonus")]
public class AdvancedChargeBonus : ScriptableObject
{
    //public string my_name;
    public Sprite icon;
    public AudioClip sfx;

    public enum AdvancedChargeBonusCostRule
    {
        and, //example: 5 red gem AND 2 green gems
        or, //example: 7 gems red or green
    }

    public AdvancedChargeBonusCostRule AdvancedChargeBonus_costRule;

    public Bonus myBonus;
    public int strength;

    //and
    public int[] targetCostByGemColor = new int[7];

    //or
    public bool[] allowedGemColors = new bool[7];
    public int targetTotal;
}
