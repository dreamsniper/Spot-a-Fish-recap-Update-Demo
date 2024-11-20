using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{
    bool menu_BonusRules = true;


    void BonusRules()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "BonusRules");

        menu_BonusRules = EditorGUILayout.Foldout(menu_BonusRules, "Bonus Rules");
        if (menu_BonusRules)
        {
            EditorGUI.indentLevel++;

                my_target.give_bonus_select = (Ruleset.give_bonus)EditorGUILayout.EnumPopup("give bonus", my_target.give_bonus_select);

                EditorGUI.indentLevel++;

                    if (my_target.give_bonus_select == Ruleset.give_bonus.no)
                        my_target.trigger_by_select = Ruleset.trigger_by.OFF;
                    else if (my_target.give_bonus_select == Ruleset.give_bonus.after_charge)
                        GiveBonus_AfterCharge();
                    else if (my_target.give_bonus_select == Ruleset.give_bonus.after_big_explosion)
                        GiveBonus_AfterBigExplosion();
                    else if (my_target.give_bonus_select == Ruleset.give_bonus.from_stage_file_or_from_gem_emitter)
                        GiveBonus_FromStageFileOfFromGemEmitter();
                    else if (my_target.give_bonus_select == Ruleset.give_bonus.advancedCharge)
                        GiveBonus_AdvancedCharge();

                    if (my_target.trigger_by_select == Ruleset.trigger_by.inventory)
                        GiveBonus_TriggerByInventory();

                EditorGUI.indentLevel--;

            //BonusGeneralRules();
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }

    /*
    void BonusGeneralRules()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "BonusGeneralRules");

        EditorGUI.indentLevel++;

        if (my_target.give_bonus_select != Ruleset.give_bonus.no)
        {
            EditorGUILayout.LabelField("General rules:");

                EditorGUI.indentLevel++;
            //if (my_target.gameLoop_selected == Ruleset.gameLoop.TurnBased || my_target.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
            if (my_target.give_bonus_select == Ruleset.give_bonus.from_stage_file_or_from_gem_emitter)
            {
                    if (my_target.lose_requirement_selected == Ruleset.lose_requirement.timer)
                        {
                        if (my_target.add_time_bonus < 1)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.add_time_bonus = EditorGUILayout.FloatField("How many seconds give Time Bonus", my_target.add_time_bonus);

                        GUI.color = Color.white;
                        }

                    if (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
                     {
                        if (my_target.add_moves_bonus < 1)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.add_moves_bonus = EditorGUILayout.IntField("How many moves give Move Bonus", my_target.add_moves_bonus);
                        GUI.color = Color.white;
                     }

                if (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero || my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
                        {
                        my_target.use_heal_cost_a_turn = EditorGUILayout.Toggle("Heal cost a turn", my_target.use_heal_cost_a_turn);
                        my_target.use_damage_cost_a_turn = EditorGUILayout.Toggle("Damage cost a turn", my_target.use_damage_cost_a_turn);
                        }
                }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("linear explosion stop against:");
                EditorGUI.indentLevel++;
                my_target.linear_explosion_stop_against_empty_space = EditorGUILayout.Toggle("empty space", my_target.linear_explosion_stop_against_empty_space);
                my_target.linear_explosion_stop_against_block = EditorGUILayout.Toggle("block", my_target.linear_explosion_stop_against_block);
                my_target.linear_explosion_stop_against_bonus = EditorGUILayout.Toggle("bonus", my_target.linear_explosion_stop_against_bonus);
                my_target.linear_explosion_stop_against_token = EditorGUILayout.Toggle("token", my_target.linear_explosion_stop_against_token);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
        }
        
        EditorGUI.indentLevel--;


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }*/
    
    

}
