using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{

    void ReachTargetScoreInfo()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "ReachTargetScoreInfo");


        if (my_target.win_requirement_selected != Ruleset.win_requirement.reach_target_score)
            return;

        EditorGUI.indentLevel++;
        my_target.threeStarScore_onWinRequirement = EditorGUILayout.Toggle("Use 3 stars rating", my_target.threeStarScore_onWinRequirement);

        if (my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
            EditorGUILayout.LabelField("WARNING! You can't use the -Use 3 stars rating- in the Win requirement AND in the Lose requirement simultaneously!");

        if (my_target.threeStarScore_onWinRequirement && !my_target.threeStarScore_onLoseRequirement)
        {
            for (int i = 0; i < my_target.player.target_score.Length; i++)
            {
                if (my_target.player.target_score[i] <= 0)
                    GUI.color = Color.red;
                else if (i > 0 && my_target.player.target_score[i] <= my_target.player.target_score[i - 1])
                {
                    GUI.color = Color.red;
                    //EditorGUILayout.LabelField("WARNING! This target score MUST BE GREATER THAN the previous one!");
                }
                else if (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score && my_target.player.target_score != my_target.enemies[0].target_score)
                    GUI.color = Color.yellow;
                else
                    GUI.color = Color.white;
                my_target.player.target_score[i] = EditorGUILayout.IntField("Star " + (i + 1).ToString() + " - Player target score", my_target.player.target_score[i]);
                GUI.color = Color.white;
            }
        }
        else
        {
            if (my_target.player.target_score[2] <= 0)
                GUI.color = Color.red;
            else if (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score && my_target.player.target_score != my_target.enemies[0].target_score)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            my_target.player.target_score[2] = EditorGUILayout.IntField("Player target score", my_target.player.target_score[2]);
            GUI.color = Color.white;
        }

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

}
