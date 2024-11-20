using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class Character  {

    public string myName;
    public Sprite myAvatar;

    //armor
    public enum gemColorArmor
    {
        weak,   // = damage * 2
        normal, // = damage * 1
        strong, // = damage * 0.5
        immune, // = damage = 0
        absorb,
        repel

    }
    public gemColorArmor[] armor;

    public int maxHp;
    public int currentHp;
    public int previousHp;

    public int score;

    public int[] previous_exploded_color = { -1, -1 };//[0] = with main gem; [1] = with minor gem
    public int explode_same_color_n_turn;

    public int[] totalNumberOfGemsDestroyed;
    public int[] numberOfGemsCollect;
    public int[] additionalGemsToCollecForStarScore;
    public bool[] thisGemColorIsCollected;
    public int totalNumberOfGemsRemaining;
    public int totalNumberOfGemsRequiredColletted;
    public int totalNumberOfExtraGemsCollettedAfterTheRequired;

    public int currentMovesLeft;

    public int currentChainLenght;//count how many moves you have done, without pass the turn to the opponent

    //bonus
    public Bonus[] bonus_slot = new Bonus[Enum.GetNames(typeof(Bonus)).Length];
    //inventory bonus
    public int[] bonus_inventory;
    public int bonus_slot_availables;
    //charge bonus
    public int[] charge_bonus_cost = new int[7];
    public int heal_me_hp_bonus;
    public int damage_opponent_bonus;
    //advance charge bonus
    public List<AdvancedChargeBonus> advancedChargeBonuses;
    public int[] gemColorAdvancedChargeBonusPool = new int[7];//count how many gem are collected and so can be spend to trigger the bonus 

    //bonus public variables that not must be touch in inspector:
    public bool[] bonus_ready;
        public int[] filling_bonus;


    //win requirements
    public int[] target_score = new int[3];
    public int[] number_of_gems_to_destroy_to_win = new int[7];

    //AI
    public Board_C.enemy_AI AI_selected = Board_C.enemy_AI.none;
    public int chance_of_use_best_move;
        //just deal damage
            public int justDealDamage_min;
            public int justDealDamage_max;
        //by_hand_setup
            public Board_C.enemy_AI_manual_setup[] temp_enemy_AI_preference_order;
        //bonus
            public int chance_of_use_bonus;
            public int chance_of_use_best_bonus;
        //advancedAI (range value 0.0 to 1.0)
                public float howMuchImportantIs_GainATurn = 1.0f;
                public float howMuchImportantIs_ChargeBonuses = 1.0f;
            //collect gems
                public float howMuchImportantIs_CollectGems = 1.0f;
                public float howMuchImportantIs_CollectGems_need_by_the_opponent = 1.0f;
            //battle
                public float howMuchImportantIs_DealDamage = 1.0f;
                public float howMuchImportantIs_AvoidDamage = 1.0f;
                public float howMuchImportantIs_HealMe = 1.0f;
                public float howMuchImportantIs_NotHealThePlayer = 1.0f;
                
}
