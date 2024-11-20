using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{
    bool show_player_inventory_bonus = true;
    bool show_enemy_inventory_bonus = true;

    void GiveBonus_TriggerByInventory_CharacterSetup(Character thisCharacter)
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_TriggerByInventory_CharacterSetup");

        EditorGUI.indentLevel++;

        //if (thisCharacter.bonus_inventory == null)
            //thisCharacter.bonus_inventory = new int[Enum.GetNames(typeof(Bonus)).Length];

        for (int i = 1; i < Enum.GetNames(typeof(Bonus)).Length; i++)
        {

            if (thisCharacter.bonus_inventory[i] < 0)
                GUI.color = Color.red;
            else
                GUI.color = Color.white;

            if (i < 8)
                thisCharacter.bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Bonus), i), thisCharacter.bonus_inventory[i]);
            else
            {
                if ((i == 8) && (my_target.lose_requirement_selected == Ruleset.lose_requirement.timer))
                {
                    EditorGUILayout.BeginHorizontal();
                    thisCharacter.bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Bonus), i), thisCharacter.bonus_inventory[i]);
                    //my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds", my_target.add_time_bonus);

                    thisCharacter.bonus_inventory[9] = 0;
                    thisCharacter.bonus_inventory[10] = 0;
                    EditorGUILayout.EndHorizontal();
                }
                else if ((i == 9) && (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves))
                {
                    EditorGUILayout.BeginHorizontal();
                    thisCharacter.bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Bonus), i), thisCharacter.bonus_inventory[i]);
                    //my_target.add_moves_bonus = EditorGUILayout.IntField("add moves", my_target.add_moves_bonus);

                    thisCharacter.bonus_inventory[8] = 0;
                    thisCharacter.bonus_inventory[10] = 0;
                    EditorGUILayout.EndHorizontal();
                }
                else if ((i == 10) && (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero))
                {
                    EditorGUILayout.BeginHorizontal();
                    thisCharacter.bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Bonus), i), thisCharacter.bonus_inventory[i]);
                    thisCharacter.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", thisCharacter.heal_me_hp_bonus);

                    thisCharacter.bonus_inventory[8] = 0;
                    thisCharacter.bonus_inventory[9] = 0;
                    EditorGUILayout.EndHorizontal();
                }
                else if ((i == 11) && (my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero))
                {
                    EditorGUILayout.BeginHorizontal();
                    thisCharacter.bonus_inventory[i] = EditorGUILayout.IntField(Enum.GetName(typeof(Bonus), i), thisCharacter.bonus_inventory[i]);
                    thisCharacter.damage_opponent_bonus = EditorGUILayout.IntField("damge HP", thisCharacter.damage_opponent_bonus);

                    thisCharacter.bonus_inventory[8] = 0;
                    thisCharacter.bonus_inventory[9] = 0;
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUI.color = Color.white;

        }
        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }

    void GiveBonus_TriggerByInventory()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_TriggerByInventory");


        EditorGUI.indentLevel++;

        show_player_inventory_bonus = EditorGUILayout.Foldout(show_player_inventory_bonus, "Player start the stage with these amount of bonuses:");
        if (show_player_inventory_bonus)
        {
            GiveBonus_TriggerByInventory_CharacterSetup(my_target.player);
        }

        if (
              (my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero)
            || (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems)
            || (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score))
        {
            show_enemy_inventory_bonus = EditorGUILayout.Foldout(show_enemy_inventory_bonus, "Enemy start the stage with these amount of bonuses:");
            if (show_enemy_inventory_bonus)
            {
                GiveBonus_TriggerByInventory_CharacterSetup(my_target.enemies[0]);
            }
        }
        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }
}
