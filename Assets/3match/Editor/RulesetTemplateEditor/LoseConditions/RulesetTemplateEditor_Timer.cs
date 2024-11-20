using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{

    void Timer()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Timer");


        if (my_target.lose_requirement_selected != Ruleset.lose_requirement.timer)
            return;

        EditorGUI.indentLevel++;

        if (my_target.timer <= 0)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.timer = EditorGUILayout.FloatField("Time in seconds", my_target.timer);
        GUI.color = Color.white;

        my_target.threeStarScore_onLoseRequirement = EditorGUILayout.Toggle("Use 3 stars rating", my_target.threeStarScore_onLoseRequirement);

        if (my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
            EditorGUILayout.LabelField("WARNING! You can't use the -Use 3 stars rating- in the Win requirement AND in the Lose requirement simultaneously!");
        else if (!my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Percentual time to spare to gain stars");
            EditorGUI.indentLevel++;
            for (int i = 0; i < my_target.percentualTimeSparedForThreeStarScore.Length; i++)
            {
                if (i > 0)
                {
                    if (my_target.percentualTimeSparedForThreeStarScore[i] <= my_target.percentualTimeSparedForThreeStarScore[i - 1])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                }

                my_target.percentualTimeSparedForThreeStarScore[i] = EditorGUILayout.Slider("Star " + (i + 1).ToString() + " :", my_target.percentualTimeSparedForThreeStarScore[i], 1, 99);
                GUI.color = Color.white;

            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }


        EditorGUILayout.LabelField("time bonus for:");
        EditorGUI.indentLevel++;
        if (my_target.tile_gift_time < 0)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.tile_gift_time = EditorGUILayout.FloatField("tile damage", my_target.tile_gift_time);
        GUI.color = Color.white;

        if (my_target.time_bonus_for_gem_explosion < 0)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.time_bonus_for_gem_explosion = EditorGUILayout.FloatField("gem explosion", my_target.time_bonus_for_gem_explosion);
        GUI.color = Color.white;

        if (my_target.time_bonus_for_secondary_explosion < 0)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.time_bonus_for_secondary_explosion = EditorGUILayout.FloatField("secondary explosion", my_target.time_bonus_for_secondary_explosion);
        GUI.color = Color.white;
        EditorGUI.indentLevel--;


        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
