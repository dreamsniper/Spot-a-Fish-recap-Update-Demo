using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{

    bool show_player_charge_bonus = true;
    bool show_enemy_charge_bonus = true;

    void GiveBonus_AfterCharge_CharacterSetup(Character thisCharacter)
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_AfterCharge_CharacterSetup");


        thisCharacter.bonus_slot_availables = EditorGUILayout.IntSlider("n. slot availables", thisCharacter.bonus_slot_availables, 0, my_target.gem_length);
        EditorGUI.indentLevel++;
        for (int i = 0; i < thisCharacter.bonus_slot_availables; i++)
        {

            EditorGUILayout.BeginHorizontal();
            thisCharacter.bonus_slot[i] = (Bonus)EditorGUILayout.EnumPopup("B. " + i, thisCharacter.bonus_slot[i]);
            if (thisCharacter.charge_bonus_cost[i] <= 0)
                thisCharacter.charge_bonus_cost[i] = 1;
            thisCharacter.charge_bonus_cost[i] = EditorGUILayout.IntField(i + " gems cost", thisCharacter.charge_bonus_cost[i]);
            EditorGUILayout.EndHorizontal();


            /*if (thisCharacter.bonus_slot[i] == Bonus.give_more_time)
            {
                EditorGUI.indentLevel++;
                if (my_target.add_time_bonus < 1)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.add_time_bonus = EditorGUILayout.FloatField("How much seconds?", my_target.add_time_bonus);
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            else if (thisCharacter.bonus_slot[i] == Bonus.give_more_moves)
            {
                EditorGUI.indentLevel++;
                if (my_target.add_moves_bonus < 1)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.add_moves_bonus = EditorGUILayout.IntField("How much moves?", my_target.add_moves_bonus);
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }*/
            if (thisCharacter.bonus_slot[i] == Bonus.HealMe)
            {
                EditorGUI.indentLevel++;
                if (thisCharacter.heal_me_hp_bonus < 1)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                thisCharacter.heal_me_hp_bonus = EditorGUILayout.IntField("How much heal?", thisCharacter.heal_me_hp_bonus);
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            else if (thisCharacter.bonus_slot[i] == Bonus.DamageOpponent)
            {
                EditorGUI.indentLevel++;
                if (thisCharacter.damage_opponent_bonus < 1)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                thisCharacter.damage_opponent_bonus = EditorGUILayout.IntField("How much damage?", thisCharacter.damage_opponent_bonus);
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
        }
        EditorGUI.indentLevel--;
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }

    void GiveBonus_AfterCharge()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_AfterCharge");


        EditorGUI.indentLevel++;

        my_target.trigger_by_select = Ruleset.trigger_by.OFF;

        show_player_charge_bonus = EditorGUILayout.Foldout(show_player_charge_bonus, "player slot bonus");
        if (show_player_charge_bonus)
        {
            GiveBonus_AfterCharge_CharacterSetup(my_target.player);
            
        }

        if (
            (my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
            || (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
            || (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score))
        {
            show_enemy_charge_bonus = EditorGUILayout.Foldout(show_enemy_charge_bonus, "enemy slot bonus");
            if (show_enemy_charge_bonus)
            {
                GiveBonus_AfterCharge_CharacterSetup(my_target.enemies[0]);
            }
        }

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }

}
