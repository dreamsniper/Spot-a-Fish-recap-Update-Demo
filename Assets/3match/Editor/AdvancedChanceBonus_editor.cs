using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AdvancedChargeBonus))]
public class AdvancedChanceBonus_editor : Editor {

    public override void OnInspectorGUI()
    {
        AdvancedChargeBonus my_target = (AdvancedChargeBonus)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "AdvancedChargeBonus");



        //my_target.name = EditorGUILayout.TextField ("Name", my_target.name);
        my_target.icon = EditorGUILayout.ObjectField("Icon", my_target.icon, typeof(Sprite), true) as Sprite;
        my_target.sfx = EditorGUILayout.ObjectField("Sfx", my_target.sfx, typeof(AudioClip), true) as AudioClip;

        my_target.myBonus = (Bonus)EditorGUILayout.EnumPopup("Bonus effect", my_target.myBonus);

        EditorGUI.indentLevel++;
        if (my_target.myBonus == Bonus.GiveMoreMoves) 
            my_target.strength = EditorGUILayout.IntField("Moves", my_target.strength);
        else if (my_target.myBonus == Bonus.GiveMoreTime)
            my_target.strength = EditorGUILayout.IntField("Seconds", my_target.strength);
        else if (my_target.myBonus == Bonus.HealMe || my_target.myBonus == Bonus.DamageOpponent)
            my_target.strength = EditorGUILayout.IntField("HP", my_target.strength);
        EditorGUI.indentLevel--;


        EditorGUILayout.Space();
        my_target.AdvancedChargeBonus_costRule = (AdvancedChargeBonus.AdvancedChargeBonusCostRule)EditorGUILayout.EnumPopup("Rule", my_target.AdvancedChargeBonus_costRule);
        EditorGUI.indentLevel++;
        if (my_target.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
        {
            if (my_target.targetTotal < 1)
                GUI.color = Color.red;
            else
                GUI.color = Color.white;
            my_target.targetTotal = EditorGUILayout.IntField("Number of gems to collect", my_target.targetTotal);
            GUI.color = Color.white;

            EditorGUILayout.LabelField("Allowed gems:");
            EditorGUI.indentLevel++;
            bool atLeastOne = false;
            for (int i = 0; i < my_target.allowedGemColors.Length; i++)
                {
                my_target.allowedGemColors[i] = EditorGUILayout.Toggle("Gem " + i, my_target.allowedGemColors[i]);
                if (my_target.allowedGemColors[i])
                    atLeastOne = true;
                }
            if (!atLeastOne)
                EditorGUILayout.LabelField("WARNING: at least one Gem must be true!");
            EditorGUI.indentLevel--;
        }
        else if (my_target.AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
        {
            

            EditorGUILayout.LabelField("Allowed gems:");
            EditorGUI.indentLevel++;
            int sum = 0;
            bool warning = false;
            for (int i = 0; i < my_target.targetCostByGemColor.Length; i++)
            {
                if (my_target.targetCostByGemColor[i] < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;

                my_target.targetCostByGemColor[i] = EditorGUILayout.IntField("gem " + i.ToString(), my_target.targetCostByGemColor[i]);
                sum += my_target.targetCostByGemColor[i];

                GUI.color = Color.white;


            }

            if (sum <= 0)
                warning = true;
            else
                warning = false;

            if (warning)
                EditorGUILayout.LabelField("WARNING: at least one Gem taget value must be greater of zero!");

            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;




        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }
}
