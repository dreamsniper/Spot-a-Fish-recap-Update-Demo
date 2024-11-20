using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{

    void CollectGemsInfo()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "CollectGemsInfo");


        if (my_target.win_requirement_selected != Ruleset.win_requirement.collect_gems)
            return;

        EditorGUI.indentLevel++;
        int total_gem_to_collect = 0;


        for (int i = 0; i < my_target.gem_length; i++)
        {
            if (my_target.player.number_of_gems_to_destroy_to_win[i] < 0)
                GUI.color = Color.red;
            else if (my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems && my_target.player.number_of_gems_to_destroy_to_win[i] != my_target.enemies[0].number_of_gems_to_destroy_to_win[i])
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            my_target.player.number_of_gems_to_destroy_to_win[i] = EditorGUILayout.IntField("gem " + i + " needful", my_target.player.number_of_gems_to_destroy_to_win[i]);
            total_gem_to_collect += my_target.player.number_of_gems_to_destroy_to_win[i];
        }
        GUI.color = Color.white;
        if (total_gem_to_collect == 0)
            EditorGUILayout.LabelField("WARNING! The total number of gem to collect can't be zero!");

        my_target.threeStarScore_onWinRequirement = EditorGUILayout.Toggle("Use 3 stars rating", my_target.threeStarScore_onWinRequirement);

        if (my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
            EditorGUILayout.LabelField("WARNING! You can't use the -Use 3 stars rating- in the Win requirement AND in the Lose requirement simultaneously!");

        if (my_target.threeStarScore_onWinRequirement && !my_target.threeStarScore_onLoseRequirement)
        {
            EditorGUI.indentLevel++;
            if (my_target.player.additionalGemsToCollecForStarScore == null || my_target.player.additionalGemsToCollecForStarScore.Length != 3)
                my_target.player.additionalGemsToCollecForStarScore = new int[3];

            for (int i = 0; i < my_target.player.additionalGemsToCollecForStarScore.Length; i++)
            {
                if (my_target.player.additionalGemsToCollecForStarScore[i] < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.player.additionalGemsToCollecForStarScore[i] = EditorGUILayout.IntField("Star " + (i + 1).ToString() + " - additional gems", my_target.player.additionalGemsToCollecForStarScore[i]);
                GUI.color = Color.white;
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
