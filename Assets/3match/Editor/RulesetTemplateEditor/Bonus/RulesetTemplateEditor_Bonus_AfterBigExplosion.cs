using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{


    void GiveBonus_AfterBigExplosion()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GiveBonus_AfterBigExplosion");


        if (my_target.trigger_by_select == Ruleset.trigger_by.OFF)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        my_target.trigger_by_select = (Ruleset.trigger_by)EditorGUILayout.EnumPopup("trigger by", my_target.trigger_by_select);
        GUI.color = Color.white;

        my_target.choose_bonus_by_select = (Ruleset.choose_bonus_by)EditorGUILayout.EnumPopup("choose bonus by", my_target.choose_bonus_by_select);
        EditorGUI.indentLevel++;
        if (my_target.choose_bonus_by_select == Ruleset.choose_bonus_by.gem_color)
        {
            for (int i = 0; i < my_target.gem_length; i++)
            {
                if (my_target.trigger_by_select != Ruleset.trigger_by.inventory && (my_target.color_explosion_give_bonus[i] == Bonus.DestroyOne || my_target.color_explosion_give_bonus[i] == Bonus.SwitchGemTeleport))
                {
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("ERROR! *" + my_target.color_explosion_give_bonus[i].ToString() + "* work ONLY with 'trigger by = Inventory'");
                }
                else
                    GUI.color = Color.white;

                EditorGUILayout.LabelField("color " + i + " give");

                EditorGUI.indentLevel++;
                my_target.color_explosion_give_bonus[i] = (Bonus)EditorGUILayout.EnumPopup("normal explosion", my_target.color_explosion_give_bonus[i]);

                if (my_target.allow2x2Explosions)
                    my_target.ColorExplosion2x2GivesThisBonus[i] = (Bonus)EditorGUILayout.EnumPopup("2x2 explosion", my_target.ColorExplosion2x2GivesThisBonus[i]);
                EditorGUI.indentLevel--;

                /*
                if (my_target.color_explosion_give_bonus[i] == Bonus.GiveMoreMoves)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_moves_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_moves_bonus = EditorGUILayout.IntField("add moves", my_target.add_moves_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else if (my_target.color_explosion_give_bonus[i] == Bonus.GiveMoreTime)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_time_bonus <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds", my_target.add_time_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else if (my_target.color_explosion_give_bonus[i] == Bonus.HealMe)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.player.heal_me_hp_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.player.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", my_target.player.heal_me_hp_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }*/
            }

        }
        else if (my_target.choose_bonus_by_select == Ruleset.choose_bonus_by.explosion_magnitude)
        {

            EditorGUI.indentLevel++;
            if (my_target.allow2x2Explosions)
            {
                if (my_target.Explosion2x2GivesThisBonus == null || my_target.Explosion2x2GivesThisBonus.Length != 4)
                    my_target.Explosion2x2GivesThisBonus = new Bonus[4];

                EditorGUILayout.LabelField("explode 2x2 gems give:");
                EditorGUI.indentLevel++;

                string explosion2x2DirectionTempString = "";
                for (int i = 0; i < 4; i++)
                {
                    if ((my_target.trigger_by_select == Ruleset.trigger_by.free_switch || my_target.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem) && (my_target.Explosion2x2GivesThisBonus[i] == Bonus.DestroyOne || my_target.Explosion2x2GivesThisBonus[i] == Bonus.SwitchGemTeleport))
                    {
                        EditorGUILayout.LabelField("ERROR! *" + my_target.Explosion2x2GivesThisBonus[i].ToString() + "* work ONLY with 'trigger by = Inventory'");
                        GUI.color = Color.red;
                    }
                    else
                        GUI.color = Color.white;



                    if (i == 0)
                        explosion2x2DirectionTempString = "if switch UP";
                    else if (i == 1)
                        explosion2x2DirectionTempString = "if switch DOWN";
                    else if (i == 2)
                        explosion2x2DirectionTempString = "if switch LEFT";
                    else if (i == 3)
                        explosion2x2DirectionTempString = "if switch RIGHT";

                    my_target.Explosion2x2GivesThisBonus[i] = (Bonus)EditorGUILayout.EnumPopup(explosion2x2DirectionTempString, my_target.Explosion2x2GivesThisBonus[i]);
                    
                    if (my_target.big_explosion_up_give_bonus[i] == Bonus.HealMe)
                    {
                        EditorGUI.indentLevel++;
                        if (my_target.player.heal_me_hp_bonus < 1)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.white;
                        my_target.player.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", my_target.player.heal_me_hp_bonus);

                        my_target.enemies[0].heal_me_hp_bonus = my_target.player.heal_me_hp_bonus;
                        GUI.color = Color.white;
                        EditorGUI.indentLevel--;
                    }
                    GUI.color = Color.white;
                }
                EditorGUI.indentLevel--;
            }

            for (int i = 0; i < 4; i++)
            {

                EditorGUILayout.LabelField("explode " + (i + 4) + " gems give:");
                EditorGUI.indentLevel++;
                //up
                if ((my_target.trigger_by_select == Ruleset.trigger_by.free_switch || my_target.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem) && (my_target.big_explosion_up_give_bonus[i] == Bonus.DestroyOne || my_target.big_explosion_up_give_bonus[i] == Bonus.SwitchGemTeleport))
                    {
                    EditorGUILayout.LabelField("ERROR! *" + my_target.big_explosion_up_give_bonus[i].ToString() + "* work ONLY with 'trigger by = Inventory'");
                    GUI.color = Color.red;
                    }
                else
                    GUI.color = Color.white;
                my_target.big_explosion_up_give_bonus[i] = (Bonus)EditorGUILayout.EnumPopup("if switch UP", my_target.big_explosion_up_give_bonus[i]);
                /*
                if (my_target.big_explosion_up_give_bonus[i] == Bonus.GiveMoreMoves)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_moves_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_moves_bonus = EditorGUILayout.IntField("add moves", my_target.add_moves_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else if (my_target.big_explosion_up_give_bonus[i] == Bonus.GiveMoreTime)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_time_bonus <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds", my_target.add_time_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                 else */if (my_target.big_explosion_up_give_bonus[i] == Bonus.HealMe)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.player.heal_me_hp_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.player.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", my_target.player.heal_me_hp_bonus);

                    my_target.enemies[0].heal_me_hp_bonus = my_target.player.heal_me_hp_bonus;
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                GUI.color = Color.white;


                //down
                if ((my_target.trigger_by_select == Ruleset.trigger_by.free_switch || my_target.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem) && (my_target.big_explosion_down_give_bonus[i] == Bonus.DestroyOne || my_target.big_explosion_down_give_bonus[i] == Bonus.SwitchGemTeleport))
                {
                    EditorGUILayout.LabelField("ERROR! *" + my_target.big_explosion_down_give_bonus[i].ToString() + "* work ONLY with 'trigger by = Inventory'");
                    GUI.color = Color.red;
                }
                else
                    GUI.color = Color.white;
                my_target.big_explosion_down_give_bonus[i] = (Bonus)EditorGUILayout.EnumPopup("if switch DOWN", my_target.big_explosion_down_give_bonus[i]);
                /*
                if (my_target.big_explosion_down_give_bonus[i] == Bonus.GiveMoreMoves)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_moves_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_moves_bonus = EditorGUILayout.IntField("add moves", my_target.add_moves_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else if (my_target.big_explosion_down_give_bonus[i] == Bonus.GiveMoreTime)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_time_bonus <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds", my_target.add_time_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else */if (my_target.big_explosion_down_give_bonus[i] == Bonus.HealMe)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.player.heal_me_hp_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.player.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", my_target.player.heal_me_hp_bonus);

                    my_target.enemies[0].heal_me_hp_bonus = my_target.player.heal_me_hp_bonus;
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                GUI.color = Color.white;


                //left
                if ((my_target.trigger_by_select == Ruleset.trigger_by.free_switch || my_target.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem) && (my_target.big_explosion_left_give_bonus[i] == Bonus.DestroyOne || my_target.big_explosion_left_give_bonus[i] == Bonus.SwitchGemTeleport))
                {
                    EditorGUILayout.LabelField("ERROR! *" + my_target.big_explosion_left_give_bonus[i].ToString() + "* work ONLY with 'trigger by = Inventory'");
                    GUI.color = Color.red;
                }
                else
                    GUI.color = Color.white;
                my_target.big_explosion_left_give_bonus[i] = (Bonus)EditorGUILayout.EnumPopup("if switch LEFT", my_target.big_explosion_left_give_bonus[i]);
                /*
                if (my_target.big_explosion_left_give_bonus[i] == Bonus.GiveMoreMoves)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_moves_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_moves_bonus = EditorGUILayout.IntField("add moves", my_target.add_moves_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else if (my_target.big_explosion_left_give_bonus[i] == Bonus.GiveMoreTime)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_time_bonus <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds", my_target.add_time_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else */if (my_target.big_explosion_left_give_bonus[i] == Bonus.HealMe)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.player.heal_me_hp_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.player.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", my_target.player.heal_me_hp_bonus);

                    my_target.enemies[0].heal_me_hp_bonus = my_target.player.heal_me_hp_bonus;
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                GUI.color = Color.white;


                //right
                if ((my_target.trigger_by_select == Ruleset.trigger_by.free_switch || my_target.trigger_by_select == Ruleset.trigger_by.switch_adjacent_gem) && (my_target.big_explosion_right_give_bonus[i] == Bonus.DestroyOne || my_target.big_explosion_right_give_bonus[i] == Bonus.SwitchGemTeleport))
                {
                    EditorGUILayout.LabelField("ERROR! *" + my_target.big_explosion_right_give_bonus[i].ToString() + "* work ONLY with 'trigger by = Inventory'");
                    GUI.color = Color.red;
                }
                else
                    GUI.color = Color.white;
                my_target.big_explosion_right_give_bonus[i] = (Bonus)EditorGUILayout.EnumPopup("if switch RIGHT", my_target.big_explosion_right_give_bonus[i]);
                /*
                if (my_target.big_explosion_right_give_bonus[i] == Bonus.GiveMoreMoves)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_moves_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_moves_bonus = EditorGUILayout.IntField("add moves", my_target.add_moves_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else if (my_target.big_explosion_right_give_bonus[i] == Bonus.GiveMoreTime)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.add_time_bonus <= 0)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.add_time_bonus = EditorGUILayout.FloatField("add seconds", my_target.add_time_bonus);
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                else */if (my_target.big_explosion_right_give_bonus[i] == Bonus.HealMe)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.player.heal_me_hp_bonus < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.player.heal_me_hp_bonus = EditorGUILayout.IntField("add HP", my_target.player.heal_me_hp_bonus);

                    my_target.enemies[0].heal_me_hp_bonus = my_target.player.heal_me_hp_bonus;
                    GUI.color = Color.white;
                    EditorGUI.indentLevel--;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;


                
            }
            EditorGUI.indentLevel--;

        }

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }
}
