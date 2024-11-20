using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{


    void PlayerHaveZeroMoves()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "PlayerHaveZeroMoves");

        if (my_target.lose_requirement_selected != Ruleset.lose_requirement.player_have_zero_moves)
            return;

        EditorGUI.indentLevel++;

        if (my_target.max_moves <= 0)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.max_moves = EditorGUILayout.IntField("Moves", my_target.max_moves);
        GUI.color = Color.white;

        EditorGUI.indentLevel++;
        my_target.threeStarScore_onLoseRequirement = EditorGUILayout.Toggle("Use 3 stars rating", my_target.threeStarScore_onLoseRequirement);

        if (my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
            EditorGUILayout.LabelField("WARNING! You can't use the -Use 3 stars rating- in the Win requirement AND in the Lose requirement simultaneously!");
        else if (!my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Moves to spare to gain stars");
            EditorGUI.indentLevel++;
            for (int i = 0; i < my_target.percentualTimeSparedForThreeStarScore.Length; i++)
            {
                if (i > 0)
                {
                    if (my_target.movesToSparedForThreeStarScore[i] <= my_target.movesToSparedForThreeStarScore[i - 1])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                }

                my_target.movesToSparedForThreeStarScore[i] = EditorGUILayout.IntField("Star " + (i + 1).ToString() + " :", my_target.movesToSparedForThreeStarScore[i]);
                GUI.color = Color.white;

            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        GainAndLoseMovesRules();

        EditorGUI.indentLevel--;


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void GainAndLoseMovesRules()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GainMovesRules");
        EditorGUI.indentLevel++;

        my_target.lose_turn_if_bad_move = EditorGUILayout.Toggle("lost a move if bad move", my_target.lose_turn_if_bad_move);
        EditorGUILayout.LabelField("gain moves if explode:");
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
        if (my_target.gain_turn_if_explode_same_color_of_previous_move)
        {
            EditorGUI.indentLevel++;
            if (my_target.move_gained_for_explode_same_color_in_two_adjacent_turn < 0)
                GUI.color = Color.red;
            else
                GUI.color = Color.white;
            my_target.move_gained_for_explode_same_color_in_two_adjacent_turn = EditorGUILayout.IntField("give n moves:", my_target.move_gained_for_explode_same_color_in_two_adjacent_turn);
            GUI.color = Color.white;
            EditorGUI.indentLevel--;
        }
        my_target.gain_turn_if_explode_more_than_3_gems = EditorGUILayout.Toggle("more than 3 gems", my_target.gain_turn_if_explode_more_than_3_gems);
        if (my_target.gain_turn_if_explode_more_than_3_gems)
        {
            EditorGUI.indentLevel++;
            if (my_target.move_gained_when_explode_more_than_3_gems.Length < 4)
                my_target.move_gained_when_explode_more_than_3_gems = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (my_target.move_gained_when_explode_more_than_3_gems[i] < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.move_gained_when_explode_more_than_3_gems[i] = EditorGUILayout.IntField("explode " + (4 + i) + " gems give " + my_target.move_gained_when_explode_more_than_3_gems[i] + " moves", my_target.move_gained_when_explode_more_than_3_gems[i]);
            }
            GUI.color = Color.white;
            EditorGUI.indentLevel--;
        }

        if (my_target.versus && ((my_target.gain_turn_if_explode_same_color_of_previous_move) || (my_target.gain_turn_if_explode_more_than_3_gems)))
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
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }

}
