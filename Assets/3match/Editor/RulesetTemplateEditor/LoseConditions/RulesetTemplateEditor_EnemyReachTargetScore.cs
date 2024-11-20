using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{
    void EnemyReachTargetScoreInfo()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "EnemyReachTargetScoreInfo");

        if (my_target.lose_requirement_selected != Ruleset.lose_requirement.enemy_reach_target_score)
            return;

        EditorGUI.indentLevel++;
        my_target.threeStarScore_onLoseRequirement = false;

            if (my_target.enemies[0].target_score[2] <= 0)
                GUI.color = Color.red;
            else if (my_target.win_requirement_selected == Ruleset.win_requirement.reach_target_score && my_target.player.target_score != my_target.enemies[0].target_score)
                GUI.color = Color.yellow;
            else
                GUI.color = Color.white;
            my_target.enemies[0].target_score[2] = EditorGUILayout.IntField("Enemy target score", my_target.enemies[0].target_score[2]);
            GUI.color = Color.white;
       

        EditorGUI.indentLevel--;

        EnemyAI();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

}
