using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{


    void GiveBonus_FromStageFileOfFromGemEmitter()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_FromStageFileOfFromGemEmitter");


        if (my_target.trigger_by_select == Ruleset.trigger_by.OFF)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.trigger_by_select = (Ruleset.trigger_by)EditorGUILayout.EnumPopup("trigger by", my_target.trigger_by_select);
        GUI.color = Color.white;

        if (my_target.trigger_by_select != Ruleset.trigger_by.inventory)
        {
            EditorGUI.indentLevel++;
            /*
            if (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
            {
                if (my_target.add_moves_bonus < 1)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.add_moves_bonus = EditorGUILayout.IntField("add moves", my_target.add_moves_bonus);
                GUI.color = Color.white;
            }
            else if (my_target.lose_requirement_selected == Ruleset.lose_requirement.timer)
            {
                if (my_target.add_time_bonus < 1)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds", my_target.add_time_bonus);
                GUI.color = Color.white;
            }
            else*/ if (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero)
            {
                if (my_target.player.heal_me_hp_bonus < 1)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.player.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", my_target.player.heal_me_hp_bonus);
                GUI.color = Color.white;
            }
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }
}
