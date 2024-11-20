using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{

    public enum CollectGemsAI
    {
        random = 0,
        collect_gems_from_less_to_more = 1,
        collect_gems_from_more_to_less = 2,
        advancedAI = 3,
        by_hand_setup = 4
    }
    public CollectGemsAI collectGemsAISelected;


    public enum BattleAI
    {
        random = 0,
        advancedAI = 3,
        by_hand_setup = 4,
        just_deal_damage = 5

    }
    public BattleAI battleAISelected;

    public enum ReachTargetScoreAI
    {
        random = 0,
        by_hand_setup = 4
    }
    public ReachTargetScoreAI reachTargetScoreAISelected;


    void EnemyAI()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "EnemyAI");

        if (!my_target.versus)
            return;

        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        my_target.enemies[0].myName = EditorGUILayout.TextField("Name", my_target.enemies[0].myName);
        my_target.enemies[0].myAvatar = EditorGUILayout.ObjectField("Avatar", my_target.enemies[0].myAvatar, typeof(Sprite), false) as Sprite;
        EditorGUILayout.EndHorizontal();

        if (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
        {
            battleAISelected = (BattleAI)my_target.enemies[0].AI_selected;

            if ((int)battleAISelected == 0
                || (int)battleAISelected == 3
                || (int)battleAISelected == 4
                || (int)battleAISelected == 5)
                GUI.color = Color.white;
            else
                battleAISelected = BattleAI.random;

            battleAISelected = (BattleAI)EditorGUILayout.EnumPopup("Enemy AI", battleAISelected);
            my_target.enemies[0].AI_selected = (Board_C.enemy_AI)battleAISelected;

        }
        else if (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score)
        {
            reachTargetScoreAISelected = (ReachTargetScoreAI)my_target.enemies[0].AI_selected;

            if ((int)reachTargetScoreAISelected == 0
                || (int)reachTargetScoreAISelected == 4)
                GUI.color = Color.white;
            else
                reachTargetScoreAISelected = ReachTargetScoreAI.random;

            reachTargetScoreAISelected = (ReachTargetScoreAI)EditorGUILayout.EnumPopup("Enemy AI", reachTargetScoreAISelected);
            my_target.enemies[0].AI_selected = (Board_C.enemy_AI)reachTargetScoreAISelected;

        }
        else if (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
        {
            collectGemsAISelected = (CollectGemsAI)my_target.enemies[0].AI_selected;

            if ((int)collectGemsAISelected == 0
                    || (int)collectGemsAISelected == 1
                    || (int)collectGemsAISelected == 2
                    || (int)collectGemsAISelected == 3
                    || (int)collectGemsAISelected == 4)
                GUI.color = Color.white;
            else
                collectGemsAISelected = CollectGemsAI.random;

            collectGemsAISelected = (CollectGemsAI)EditorGUILayout.EnumPopup("Enemy AI", collectGemsAISelected);
            my_target.enemies[0].AI_selected = (Board_C.enemy_AI)collectGemsAISelected;

        }

        //Enemy AI controls:
        EditorGUI.indentLevel++;
        if (my_target.enemies[0].AI_selected != Board_C.enemy_AI.random && my_target.enemies[0].AI_selected != Board_C.enemy_AI.just_deal_damage)
            my_target.enemies[0].chance_of_use_best_move = EditorGUILayout.IntSlider("% use best move", my_target.enemies[0].chance_of_use_best_move, 0, 100);
        if (my_target.enemies[0].AI_selected == Board_C.enemy_AI.random)
        {
            EnemyBonusAI();
        }
        else if (my_target.enemies[0].AI_selected == Board_C.enemy_AI.collect_gems_from_less_to_more)
        {
            EnemyBonusAI();
        }
        else if (my_target.enemies[0].AI_selected == Board_C.enemy_AI.collect_gems_from_more_to_less)
        {
            EnemyBonusAI();
        }
        else if (my_target.enemies[0].AI_selected == Board_C.enemy_AI.just_deal_damage)
        {
            my_target.enemies[0].justDealDamage_min = EditorGUILayout.IntField("min damage", my_target.enemies[0].justDealDamage_min);
            if (my_target.enemies[0].justDealDamage_min < 1)
                my_target.enemies[0].justDealDamage_min = 1;

            my_target.enemies[0].justDealDamage_max = EditorGUILayout.IntField("max damage", my_target.enemies[0].justDealDamage_max);
            if (my_target.enemies[0].justDealDamage_max < my_target.enemies[0].justDealDamage_min)
                my_target.enemies[0].justDealDamage_max = my_target.enemies[0].justDealDamage_min;

            my_target.enemies[0].chance_of_use_bonus = 0;
        }
        else if (my_target.enemies[0].AI_selected == Board_C.enemy_AI.by_hand_setup)
        {

            EditorGUI.indentLevel++;

            for (int i = 0; i < my_target.gem_length; i++)
            {

                if ((int)my_target.enemies[0].temp_enemy_AI_preference_order[i] > my_target.gem_length - 1)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("ERROR! This gem color exceed the current number of 'gem colors' set in 'Gems and Emitters' ");
                }
                else
                    GUI.color = Color.white;

                my_target.enemies[0].temp_enemy_AI_preference_order[i] = (Board_C.enemy_AI_manual_setup)EditorGUILayout.EnumPopup("Priority order n. " + i, my_target.enemies[0].temp_enemy_AI_preference_order[i]);
            }
            GUI.color = Color.white;
            EditorGUI.indentLevel--;

            EnemyBonusAI();
        }
        else if (my_target.enemies[0].AI_selected == Board_C.enemy_AI.advancedAI)
            {
                EditorGUI.indentLevel++;
                EnemyBonusAI();
                EditorGUILayout.LabelField("How much important is:");
                EditorGUI.indentLevel++;

                if (my_target.gain_turn_if_explode_more_than_3_gems || my_target.gain_turn_if_explode_same_color_of_previous_move)
                    my_target.enemies[0].howMuchImportantIs_GainATurn = EditorGUILayout.Slider("gain a turn", my_target.enemies[0].howMuchImportantIs_GainATurn, 0.0f, 1.0f);

                if (my_target.give_bonus_select == Ruleset.give_bonus.after_charge || my_target.give_bonus_select == Ruleset.give_bonus.advancedCharge)
                    my_target.enemies[0].howMuchImportantIs_ChargeBonuses = EditorGUILayout.Slider("charge my bonuses", my_target.enemies[0].howMuchImportantIs_ChargeBonuses, 0.0f, 1.0f);

                if (my_target.use_armor)
                    {
                    my_target.enemies[0].howMuchImportantIs_DealDamage = EditorGUILayout.Slider("deal damage", my_target.enemies[0].howMuchImportantIs_DealDamage, 0.0f, 1.0f);
                    my_target.enemies[0].howMuchImportantIs_AvoidDamage = EditorGUILayout.Slider("avoid damage", my_target.enemies[0].howMuchImportantIs_AvoidDamage, 0.0f, 1.0f);
                    my_target.enemies[0].howMuchImportantIs_HealMe = EditorGUILayout.Slider("heal myself", my_target.enemies[0].howMuchImportantIs_HealMe, 0.0f, 1.0f);
                    my_target.enemies[0].howMuchImportantIs_NotHealThePlayer = EditorGUILayout.Slider("don't healt the player", my_target.enemies[0].howMuchImportantIs_NotHealThePlayer, 0.0f, 1.0f);
                    }
                else
                    {
                    my_target.enemies[0].howMuchImportantIs_CollectGems = EditorGUILayout.Slider("collect gems", my_target.enemies[0].howMuchImportantIs_CollectGems, 0.0f, 1.0f);
                    my_target.enemies[0].howMuchImportantIs_CollectGems_need_by_the_opponent = EditorGUILayout.Slider("collect gems need by the opponent", my_target.enemies[0].howMuchImportantIs_CollectGems_need_by_the_opponent, 0.0f, 1.0f);
                    }


            EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
        EditorGUI.indentLevel--;

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }




    void EnemyBonusAI()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "EnemyBonusAI");

        if (my_target.give_bonus_select == Ruleset.give_bonus.no)
        {
            my_target.enemies[0].chance_of_use_bonus = 0;
            my_target.enemies[0].chance_of_use_best_bonus = 0;
            return;
        }

        EditorGUI.indentLevel++;
        
        my_target.enemies[0].chance_of_use_bonus = EditorGUILayout.IntSlider("% chance of use a bonus", my_target.enemies[0].chance_of_use_bonus, 0, 100);
                            EditorGUI.indentLevel++;
                            if (my_target.enemies[0].chance_of_use_bonus > 0)
                                my_target.enemies[0].chance_of_use_best_bonus = EditorGUILayout.IntSlider("% chance of use the best bonus", my_target.enemies[0].chance_of_use_best_bonus, 0, 100);
                            EditorGUI.indentLevel--;
                            
        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
