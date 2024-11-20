using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(ThemeTemplate))]
public class ThemeTemplateEditor : Editor
{


    public override void OnInspectorGUI()
    {

        ThemeTemplate my_target = (ThemeTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "ThemeTemplate");

        Sprites();
        Fx();
        Frame();


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    public void Sprites()
    {

        ThemeTemplate my_target = (ThemeTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Sprites");

        my_target.show_s_sprites = EditorGUILayout.Foldout(my_target.show_s_sprites, "Sprites");
        if (my_target.show_s_sprites)
        {
            EditorGUI.indentLevel++;

            my_target.show_s_bonus_gui = EditorGUILayout.Foldout(my_target.show_s_bonus_gui, "Bonus_gui");
            if (my_target.show_s_bonus_gui)
            {
                EditorGUI.indentLevel++;


                for (int i = 1; i < Enum.GetNames(typeof(Bonus)).Length; i++)
                {

                    if (!my_target.gui_bonus[(int)i - 1])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.gui_bonus[(int)i - 1] = EditorGUILayout.ObjectField(Enum.GetName(typeof(Bonus), i).ToString(), my_target.gui_bonus[(int)i - 1], typeof(Sprite), false) as Sprite;

                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }


            my_target.show_s_on_board_bonus = EditorGUILayout.Foldout(my_target.show_s_on_board_bonus, "On board bonus");
            if (my_target.show_s_on_board_bonus)
            {
                EditorGUI.indentLevel++;
                for (int i = 1; i < Enum.GetNames(typeof(Bonus)).Length; i++)
                {

                    if (!my_target.on_board_bonus_sprites[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;

                    my_target.on_board_bonus_sprites[i] = EditorGUILayout.ObjectField(Enum.GetName(typeof(Bonus), i).ToString(), my_target.on_board_bonus_sprites[i], typeof(Sprite), false) as Sprite;

                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }



            my_target.show_s_tiles = EditorGUILayout.Foldout(my_target.show_s_tiles, "Tiles");
            if (my_target.show_s_tiles)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < 4; i++)
                {
                    if (!my_target.tile_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.tile_hp[i] = EditorGUILayout.ObjectField("tile hp " + i, my_target.tile_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }



            my_target.show_s_gems = EditorGUILayout.Foldout(my_target.show_s_gems, "Gems");
            if (my_target.show_s_gems)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < my_target.gem_colors.Length; i++)
                {
                    if (!my_target.gem_colors[i])
                    {
                        if (i <= 3)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.yellow;
                    }
                    else
                        GUI.color = Color.white;
                    my_target.gem_colors[i] = EditorGUILayout.ObjectField("gem n. " + i, my_target.gem_colors[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }



            my_target.show_s_padlocks = EditorGUILayout.Foldout(my_target.show_s_padlocks, "Restrains:");
            if (my_target.show_s_padlocks)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.lock_gem_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.lock_gem_hp[i] = EditorGUILayout.ObjectField("padlock hp " + (i + 1), my_target.lock_gem_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;

                EditorGUILayout.Space();

                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.ice_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.ice_hp[i] = EditorGUILayout.ObjectField("ice hp " + (i + 1), my_target.ice_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;


                EditorGUILayout.Space();

                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.fallingPadlock_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.fallingPadlock_hp[i] = EditorGUILayout.ObjectField("falling padlock hp " + (i + 1), my_target.fallingPadlock_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;

                EditorGUILayout.Space();

                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.cage_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.cage_hp[i] = EditorGUILayout.ObjectField("cage hp " + (i + 1), my_target.cage_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;


                EditorGUI.indentLevel--;
            }



            my_target.show_s_blocks = EditorGUILayout.Foldout(my_target.show_s_blocks, "Blocks");
            if (my_target.show_s_blocks)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.block_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.block_hp[i] = EditorGUILayout.ObjectField("block hp " + (i + 1), my_target.block_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;

                EditorGUILayout.Space();

                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.falling_block_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.falling_block_hp[i] = EditorGUILayout.ObjectField("falling block hp " + (i + 1), my_target.falling_block_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;

                EditorGUILayout.Space();

                for (int i = 0; i < 3; i++)
                {
                    if (!my_target.generative_block_hp[i])
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.generative_block_hp[i] = EditorGUILayout.ObjectField("generative block hp " + (i + 1), my_target.generative_block_hp[i], typeof(Sprite), false) as Sprite;
                }
                GUI.color = Color.white;

                EditorGUI.indentLevel--;
            }



            my_target.show_s_misc = EditorGUILayout.Foldout(my_target.show_s_misc, "Misc");
            if (my_target.show_s_misc)
            {
                EditorGUI.indentLevel++;

                if (!my_target.junk)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.junk = EditorGUILayout.ObjectField("junk", my_target.junk, typeof(Sprite), false) as Sprite;
                GUI.color = Color.white;

                if (!my_target.token)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.token = EditorGUILayout.ObjectField("token", my_target.token, typeof(Sprite), false) as Sprite;
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }


    public void Fx()
    {

        ThemeTemplate my_target = (ThemeTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Fx");

        my_target.show_s_fx = EditorGUILayout.Foldout(my_target.show_s_fx, "Fx");
        if (my_target.show_s_fx)
        {
            EditorGUI.indentLevel++;

            my_target.bonus_have_explosion_fx = EditorGUILayout.Toggle("Bonus fx", my_target.bonus_have_explosion_fx);
            if (my_target.bonus_have_explosion_fx)
            {
                EditorGUI.indentLevel++;

                if (my_target.destroy_one_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_one_fx = EditorGUILayout.ObjectField("destroy one", my_target.destroy_one_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_3x3_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_3x3_fx = EditorGUILayout.ObjectField("destroy 3x3", my_target.destroy_3x3_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_horizontal_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_horizontal_fx = EditorGUILayout.ObjectField("destroy horizontal", my_target.destroy_horizontal_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_vertical_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_vertical_fx = EditorGUILayout.ObjectField("destroy vertical", my_target.destroy_vertical_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                if (my_target.destroy_horizontal_and_vertical_fx == null)
                    GUI.color = Color.red;
                else
                    GUI.color = Color.white;
                my_target.destroy_horizontal_and_vertical_fx = EditorGUILayout.ObjectField("destroy horizontal and vertical", my_target.destroy_horizontal_and_vertical_fx, typeof(Transform), true) as Transform;
                GUI.color = Color.white;

                EditorGUI.indentLevel--;
            }

            my_target.gem_explosion_fx_rule_selected = (ThemeTemplate.gem_explosion_fx_rule)EditorGUILayout.EnumPopup("Gem explosion fx", my_target.gem_explosion_fx_rule_selected);
            EditorGUI.indentLevel++;
            if (my_target.gem_explosion_fx_rule_selected == ThemeTemplate.gem_explosion_fx_rule.for_each_gem)
            {
                EditorGUILayout.LabelField("gem explosion fx by color:");
                EditorGUI.indentLevel++;
                for (int i = 0; i < my_target.gem_colors.Length; i++)
                {
                    if (my_target.gem_explosion_fx[i] == null)
                    {
                        if (i <= 3)
                            GUI.color = Color.red;
                        else
                            GUI.color = Color.yellow;
                    }
                    else
                        GUI.color = Color.white;

                    
                    my_target.gem_explosion_fx[i] = EditorGUILayout.ObjectField("gem " + i, my_target.gem_explosion_fx[i], typeof(Transform), true) as Transform;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            else if (my_target.gem_explosion_fx_rule_selected == ThemeTemplate.gem_explosion_fx_rule.only_for_big_explosion)
            {
                EditorGUILayout.LabelField("gem explosion fx by explosion magnitude:");
                EditorGUI.indentLevel++;
                for (int i = 0; i < 4; i++)
                {
                    if (my_target.gem_big_explosion_fx[i] == null)
                        GUI.color = Color.red;
                    else
                        GUI.color = Color.white;
                    my_target.gem_big_explosion_fx[i] = EditorGUILayout.ObjectField("explosion " + (i + 4), my_target.gem_big_explosion_fx[i], typeof(Transform), true) as Transform;
                }
                GUI.color = Color.white;
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;

            

            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }


    public void Frame()
    {

        ThemeTemplate my_target = (ThemeTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "Frame");

        my_target.show_s_frame = EditorGUILayout.Foldout(my_target.show_s_frame, "Frame");
        if (my_target.show_s_frame)
        {
            EditorGUI.indentLevel++;

            my_target.show_frame_board_decoration = EditorGUILayout.Toggle("Frame", my_target.show_frame_board_decoration);

            my_target.frame_elements[0] = EditorGUILayout.ObjectField("up", my_target.frame_elements[0], typeof(Transform), true) as Transform;
            my_target.frame_elements[1] = EditorGUILayout.ObjectField("down", my_target.frame_elements[1], typeof(Transform), true) as Transform;

            my_target.frame_elements[2] = EditorGUILayout.ObjectField("right", my_target.frame_elements[2], typeof(Transform), true) as Transform;
            my_target.frame_elements[3] = EditorGUILayout.ObjectField("left", my_target.frame_elements[3], typeof(Transform), true) as Transform;

            my_target.frame_elements[4] = EditorGUILayout.ObjectField("corner_in_down_R", my_target.frame_elements[4], typeof(Transform), true) as Transform;
            my_target.frame_elements[5] = EditorGUILayout.ObjectField("corner_in_down_L", my_target.frame_elements[5], typeof(Transform), true) as Transform;
            my_target.frame_elements[6] = EditorGUILayout.ObjectField("corner_in_up_R", my_target.frame_elements[6], typeof(Transform), true) as Transform;
            my_target.frame_elements[7] = EditorGUILayout.ObjectField("corner_in_up_L", my_target.frame_elements[7], typeof(Transform), true) as Transform;

            my_target.frame_elements[8] = EditorGUILayout.ObjectField("corner_out_down_R", my_target.frame_elements[8], typeof(Transform), true) as Transform;
            my_target.frame_elements[9] = EditorGUILayout.ObjectField("corner_out_down_L", my_target.frame_elements[9], typeof(Transform), true) as Transform;
            my_target.frame_elements[10] = EditorGUILayout.ObjectField("corner_out_up_R", my_target.frame_elements[10], typeof(Transform), true) as Transform;
            my_target.frame_elements[11] = EditorGUILayout.ObjectField("corner_out_up_L", my_target.frame_elements[11], typeof(Transform), true) as Transform;

            my_target.frame_elements[12] = EditorGUILayout.ObjectField("U_R", my_target.frame_elements[12], typeof(Transform), true) as Transform;
            my_target.frame_elements[13] = EditorGUILayout.ObjectField("U_L", my_target.frame_elements[13], typeof(Transform), true) as Transform;
            my_target.frame_elements[14] = EditorGUILayout.ObjectField("U_up", my_target.frame_elements[14], typeof(Transform), true) as Transform;
            my_target.frame_elements[15] = EditorGUILayout.ObjectField("U_down", my_target.frame_elements[15], typeof(Transform), true) as Transform;

            my_target.frame_elements[16] = EditorGUILayout.ObjectField("O", my_target.frame_elements[16], typeof(Transform), true) as Transform;

            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
