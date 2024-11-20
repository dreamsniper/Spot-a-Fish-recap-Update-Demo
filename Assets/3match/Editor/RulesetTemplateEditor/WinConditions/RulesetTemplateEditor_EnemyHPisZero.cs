using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{
    bool enemy_armor_battle_info = true;

    void EnemyHpIsZero()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "CollectGemsInfo");


        if (my_target.win_requirement_selected != Ruleset.win_requirement.enemy_hp_is_zero)
            return;

        EditorGUI.indentLevel++;
        EditorGUI.indentLevel++;

        enemy_armor_battle_info = EditorGUILayout.Foldout(enemy_armor_battle_info, "Enemy info");
        if (enemy_armor_battle_info)
        {
            ArmorBattle_CharacterInfo(my_target.enemies[0]);

            EditorGUI.indentLevel--;
            EnemyAI();
            EditorGUI.indentLevel++;
        }

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;



        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

}
