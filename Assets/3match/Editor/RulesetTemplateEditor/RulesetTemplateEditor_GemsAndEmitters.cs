using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{
    bool menu_GemsRules = true;
    bool show_gem_explosion_outcome = true;


    void GemsRules()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GemsRules");


        menu_GemsRules = EditorGUILayout.Foldout(menu_GemsRules, "Gems and Emitters");
        if (menu_GemsRules)
        {
            EditorGUI.indentLevel++;
            my_target.gem_length = EditorGUILayout.IntSlider("colors", my_target.gem_length, 4, 7);


            GemExplosionOutcome();

            my_target.diagonal_falling = EditorGUILayout.Toggle("allow diagonal falling", my_target.diagonal_falling);
            my_target.allow2x2Explosions = EditorGUILayout.Toggle("allow 2x2 match explosion", my_target.allow2x2Explosions);

            my_target.gem_emitter_rule = (Ruleset.gem_emitter)EditorGUILayout.EnumPopup("gem emitters", my_target.gem_emitter_rule);
            if (my_target.gem_emitter_rule == Ruleset.gem_emitter.special)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("create a special element each n gems created:");
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                if (my_target.create_a_special_element_each_n_gems_created_min < 0)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;

                my_target.create_a_special_element_each_n_gems_created_min = EditorGUILayout.IntField("min", my_target.create_a_special_element_each_n_gems_created_min);
                GUI.color = Color.white;

                if (my_target.create_a_special_element_each_n_gems_created_max < my_target.create_a_special_element_each_n_gems_created_min)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;

                my_target.create_a_special_element_each_n_gems_created_max = EditorGUILayout.IntField("max", my_target.create_a_special_element_each_n_gems_created_max);
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                my_target.chance_to_create_a_special_element = EditorGUILayout.IntSlider("% chance to create a special element", my_target.chance_to_create_a_special_element, 1, 100);
                EditorGUI.indentLevel++;
                my_target.reset_gem_creation_count_if_chance_to_create_a_special_element_fail = EditorGUILayout.Toggle("reset gem creation count if chance to create a special element fail", my_target.reset_gem_creation_count_if_chance_to_create_a_special_element_fail);
                my_target.max_number_of_specials_on_board_at_the_same_time = EditorGUILayout.IntField("max number of specials on board at the same time", my_target.max_number_of_specials_on_board_at_the_same_time);
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("creation chances weights:");
                EditorGUI.indentLevel++;
                my_target.token_creation_chance_weight = EditorGUILayout.IntField("token", my_target.token_creation_chance_weight);
                if (my_target.token_creation_chance_weight > 0)
                {
                    EditorGUI.indentLevel++;
                    if (my_target.number_of_token_to_emit < 1)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.number_of_token_to_emit = EditorGUILayout.IntField("number of tokens to emit", my_target.number_of_token_to_emit);
                    GUI.color = Color.white;
                    my_target.max_number_of_tokens_on_board_at_the_same_time = EditorGUILayout.IntSlider("max number on board at the same time", my_target.max_number_of_tokens_on_board_at_the_same_time, 1, my_target.number_of_token_to_emit);
                    my_target.emit_token_only_after_all_tiles_are_destroyed = EditorGUILayout.Toggle("emit token only after all tiles are destroyed", my_target.emit_token_only_after_all_tiles_are_destroyed);
                    EditorGUI.indentLevel--;
                }
                my_target.junk_creation_chance_weight = EditorGUILayout.IntField("junk", my_target.junk_creation_chance_weight);
                    EditorGUI.indentLevel++;
                    my_target.max_number_of_junks_on_board_at_the_same_time = EditorGUILayout.IntField("max number on board at the same time", my_target.max_number_of_junks_on_board_at_the_same_time);
                    EditorGUI.indentLevel--;

                foreach (Bonus i in Enum.GetValues(typeof(Bonus)))
                {
                    if (i == 0)
                        continue;

                    my_target.bonus_creation_chances_weight[(int)i] = EditorGUILayout.IntField(i.ToString(), my_target.bonus_creation_chances_weight[(int)i]);
                }
                my_target.max_number_of_bonuses_on_board_at_the_same_time = EditorGUILayout.IntField("max number of bonuses on board at the same time", my_target.max_number_of_bonuses_on_board_at_the_same_time);
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }


    void GemExplosionOutcome()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "GemExplosionOutcome");

        if (!my_target.use_armor)
            return;

        EditorGUI.indentLevel++;

        show_gem_explosion_outcome = EditorGUILayout.Foldout(show_gem_explosion_outcome, "Gem explosion outcome:");
                    if (show_gem_explosion_outcome)
                    {
                        EditorGUI.indentLevel++;

                        int gem_damage_opponent_max_value = 0;
                        for (int i = 0; i< my_target.gem_length; i++)
                        {
                            EditorGUILayout.LabelField("Gem["+i+"]");
                            EditorGUI.indentLevel++;
                                my_target.gemExplosionOutcomes[i].damageOpponent = EditorGUILayout.IntField("damage opponent", my_target.gemExplosionOutcomes[i].damageOpponent);

                                if (my_target.gemExplosionOutcomes[i].damageOpponent< 0)
                                    my_target.gemExplosionOutcomes[i].damageOpponent = 0;

                                if (gem_damage_opponent_max_value<my_target.gemExplosionOutcomes[i].damageOpponent)
                                    gem_damage_opponent_max_value = my_target.gemExplosionOutcomes[i].damageOpponent;



                                my_target.gemExplosionOutcomes[i].damageMe = EditorGUILayout.IntField("damage me", my_target.gemExplosionOutcomes[i].damageMe);

                                if (my_target.gemExplosionOutcomes[i].damageMe< 0)
                                    my_target.gemExplosionOutcomes[i].damageMe = 0;


                            my_target.gemExplosionOutcomes[i].healMe = EditorGUILayout.IntField("heal me", my_target.gemExplosionOutcomes[i].healMe);

                            if (my_target.gemExplosionOutcomes[i].healMe< 0)
                                my_target.gemExplosionOutcomes[i].healMe = 0;
                            EditorGUI.indentLevel--;
                        }
                        if (gem_damage_opponent_max_value <= 0 && (my_target.lose_requirement_selected == Ruleset.lose_requirement.player_hp_is_zero || my_target.win_requirement_selected == Ruleset.win_requirement.enemy_hp_is_zero))
                            EditorGUILayout.LabelField("WARNING!!! At least one -damage opponent- MUST be greather than 0");

                        EditorGUI.indentLevel--;
                    }

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
