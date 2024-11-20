using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{
    bool player_armor_battle_info = true;

    void PlayerHpIsZero()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "PlayerHpIsZero");


        if (my_target.lose_requirement_selected != Ruleset.lose_requirement.player_hp_is_zero)
            return;

        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;

        player_armor_battle_info = EditorGUILayout.Foldout(player_armor_battle_info, "Player info");
        if (player_armor_battle_info)
            ArmorBattle_CharacterInfo(my_target.player);



        EditorGUI.indentLevel--;

        if (my_target.tile_gift_time < 0)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.tile_gift_hp = EditorGUILayout.IntField("tile damage give HP", my_target.tile_gift_hp);
        GUI.color = Color.white;



        my_target.threeStarScore_onLoseRequirement = EditorGUILayout.Toggle("Use 3 stars rating", my_target.threeStarScore_onLoseRequirement);

        if (my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
            EditorGUILayout.LabelField("WARNING! You can't use the -Use 3 stars rating- in the Win requirement AND in the Lose requirement simultaneously!");
        else if (!my_target.threeStarScore_onWinRequirement && my_target.threeStarScore_onLoseRequirement)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Percentual HP to spare to gain stars");
            EditorGUI.indentLevel++;
            for (int i = 0; i < my_target.percentualPlayerHPSparedForStarScore.Length; i++)
            {
                if (i > 0)
                {
                    if (my_target.percentualPlayerHPSparedForStarScore[i] <= my_target.percentualPlayerHPSparedForStarScore[i - 1])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                }

                my_target.percentualPlayerHPSparedForStarScore[i] = EditorGUILayout.IntSlider("Star " + (i + 1).ToString() + " :", my_target.percentualPlayerHPSparedForStarScore[i], 0, 99);
                GUI.color = Color.white;

            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }


        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

}
