using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{


    void TurnRules()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "TurnRules");

        if (my_target.versus || my_target.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
        {
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            my_target.lose_turn_if_bad_move = EditorGUILayout.Toggle("lost turn if bad move", my_target.lose_turn_if_bad_move);
            EditorGUILayout.LabelField("gain a turn if explode:");
            EditorGUI.indentLevel++;

            my_target.gain_turn_if_secondary_explosion = EditorGUILayout.Toggle("secondary explosion", my_target.gain_turn_if_secondary_explosion);
            if (my_target.gain_turn_if_secondary_explosion)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.seconday_explosion_maginiture_needed_to_gain_a_turn < 3)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.seconday_explosion_maginiture_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum magnitude requested", my_target.seconday_explosion_maginiture_needed_to_gain_a_turn);
                    GUI.color = Color.white;

                    if (my_target.combo_lenght_needed_to_gain_a_turn < 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.combo_lenght_needed_to_gain_a_turn = EditorGUILayout.IntField("minimum combo lenght requested", my_target.combo_lenght_needed_to_gain_a_turn);
                    GUI.color = Color.white;

                    EditorGUI.indentLevel--;
                }

            my_target.gain_turn_if_explode_same_color_of_previous_move = EditorGUILayout.Toggle("same color of your previous move", my_target.gain_turn_if_explode_same_color_of_previous_move);
            my_target.gain_turn_if_explode_more_than_3_gems = EditorGUILayout.Toggle("more than 3 gems", my_target.gain_turn_if_explode_more_than_3_gems);

            if (my_target.gain_turn_if_explode_same_color_of_previous_move || my_target.gain_turn_if_explode_more_than_3_gems)
                {
                    EditorGUILayout.BeginHorizontal();
                    my_target.chain_turns_limit = EditorGUILayout.Toggle("chain limit", my_target.chain_turns_limit);
                    if (my_target.chain_turns_limit)
                    {
                        if (my_target.max_chain_turns <= 0)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.max_chain_turns = EditorGUILayout.IntField("max", my_target.max_chain_turns);
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }



}
