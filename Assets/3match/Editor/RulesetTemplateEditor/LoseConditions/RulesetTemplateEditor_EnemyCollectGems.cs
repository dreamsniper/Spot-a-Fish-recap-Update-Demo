using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{

    void EnemyCollectGems()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "EnemyCollectGems");


        if (my_target.lose_requirement_selected != Ruleset.lose_requirement.enemy_collect_gems)
            return;

        EditorGUI.indentLevel++;
        int total_gem_to_collect = 0;
        for (int i = 0; i < my_target.gem_length; i++)
        {
            if (my_target.enemies[0].number_of_gems_to_destroy_to_win[i] < 0)
                GUI.color = Color.red;
            else if (my_target.win_requirement_selected == Ruleset.win_requirement.collect_gems && my_target.player.number_of_gems_to_destroy_to_win[i] != my_target.enemies[0].number_of_gems_to_destroy_to_win[i])
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            my_target.enemies[0].number_of_gems_to_destroy_to_win[i] = EditorGUILayout.IntField("gem " + i + " needful", my_target.enemies[0].number_of_gems_to_destroy_to_win[i]);
            total_gem_to_collect += my_target.enemies[0].number_of_gems_to_destroy_to_win[i];
        }
        GUI.color = Color.white;
        if (total_gem_to_collect == 0)
            EditorGUILayout.LabelField("WARNING! The total number of gem to collect can't be zero!");

        EditorGUI.indentLevel--;

        EnemyAI();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

}
