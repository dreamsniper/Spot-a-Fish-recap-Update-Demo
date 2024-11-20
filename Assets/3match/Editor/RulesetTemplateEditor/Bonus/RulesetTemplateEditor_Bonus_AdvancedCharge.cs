using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{
    void GiveBonus_AdvancedCharge_CharacterSetup(Character thisCharacter)
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_advancedCharge_CharacterSetup");


        if (thisCharacter.advancedChargeBonuses == null)
        {
            thisCharacter.advancedChargeBonuses = new System.Collections.Generic.List<AdvancedChargeBonus>();
            thisCharacter.advancedChargeBonuses.Add(new AdvancedChargeBonus());
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("");
        if (GUILayout.Button("Add new bonus", GUILayout.Width(200)))
        {
            thisCharacter.advancedChargeBonuses.Add(null);
            Debug.Log("count: " + thisCharacter.advancedChargeBonuses.Count);
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < thisCharacter.advancedChargeBonuses.Count; i++)
        {
            if (thisCharacter.advancedChargeBonuses[i] == null)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.red;
                thisCharacter.advancedChargeBonuses[i] = EditorGUILayout.ObjectField("*" + i.ToString() + "* " + "Put a bonus here", thisCharacter.advancedChargeBonuses[i], typeof(AdvancedChargeBonus), true) as AdvancedChargeBonus;
                GUI.color = Color.white;
                if (GUILayout.Button("Delete"))
                    thisCharacter.advancedChargeBonuses.RemoveAt(i);
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("*" + i.ToString() + "* " + thisCharacter.advancedChargeBonuses[i].name + ": " + thisCharacter.advancedChargeBonuses[i].myBonus.ToString());

                if (thisCharacter.advancedChargeBonuses[i].myBonus == Bonus.DamageOpponent || thisCharacter.advancedChargeBonuses[i].myBonus == Bonus.GiveMoreMoves || thisCharacter.advancedChargeBonuses[i].myBonus == Bonus.GiveMoreTime || thisCharacter.advancedChargeBonuses[i].myBonus == Bonus.HealMe)
                    GUILayout.Label("[" + thisCharacter.advancedChargeBonuses[i].strength + "]");

                thisCharacter.advancedChargeBonuses[i] = EditorGUILayout.ObjectField(thisCharacter.advancedChargeBonuses[i], typeof(AdvancedChargeBonus), true) as AdvancedChargeBonus;


                if (GUILayout.Button("Delete"))
                    thisCharacter.advancedChargeBonuses.RemoveAt(i);

                GUILayout.EndHorizontal();

                if (thisCharacter.advancedChargeBonuses[i] != null)
                {
                    EditorGUI.indentLevel++;
                    GUILayout.BeginHorizontal();
                

                    if (thisCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.or)
                    {
                        EditorGUILayout.LabelField("Target OR = " + thisCharacter.advancedChargeBonuses[i].targetTotal.ToString());
                        for (int g = 0; g < thisCharacter.advancedChargeBonuses[i].allowedGemColors.Length; g++)
                        {
                            if (thisCharacter.advancedChargeBonuses[i].allowedGemColors[g])
                                GUILayout.Label("gem " + g.ToString());
                        }
                    }
                    if (thisCharacter.advancedChargeBonuses[i].AdvancedChargeBonus_costRule == AdvancedChargeBonus.AdvancedChargeBonusCostRule.and)
                    {
                        EditorGUILayout.LabelField("Target AND = ");
                        for (int g = 0; g < thisCharacter.advancedChargeBonuses[i].targetCostByGemColor.Length; g++)
                        {
                            if (thisCharacter.advancedChargeBonuses[i].targetCostByGemColor[g] > 0)
                                GUILayout.Label("gem " + g.ToString() + " = " + thisCharacter.advancedChargeBonuses[i].targetCostByGemColor[g].ToString());
                        }
                    }
                    GUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.Space();
        }


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }



    void GiveBonus_AdvancedCharge()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_advancedCharge");

        my_target.trigger_by_select = Ruleset.trigger_by.OFF;

        EditorGUI.indentLevel++;


        my_target.allTheBonusesShareTheSameGemPool = EditorGUILayout.Toggle("all the bonuses share the same GemPool", my_target.allTheBonusesShareTheSameGemPool);

        show_player_charge_bonus = EditorGUILayout.Foldout(show_player_charge_bonus, "player slot bonus");
        if (show_player_charge_bonus)
        {
            GiveBonus_AdvancedCharge_CharacterSetup(my_target.player);
        }

        if (my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero || my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_collect_gems || my_target.lose_requirement_selected == Ruleset.lose_requirement.enemy_reach_target_score)
        {
            show_enemy_charge_bonus = EditorGUILayout.Foldout(show_enemy_charge_bonus, "enemy bonus");
            if (show_enemy_charge_bonus)
            {
                GiveBonus_AdvancedCharge_CharacterSetup(my_target.enemies[0]);
            }
        }

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }
}
