using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{
 


    void ArmorBattle_CharacterInfo(Character thisCharacter)
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "ArmorBattle_CharacterInfo");

        if (thisCharacter.maxHp <= 0)
            GUI.color = Color.red;
        else
            GUI.color = Color.white;
        thisCharacter.maxHp = EditorGUILayout.IntField("HP", thisCharacter.maxHp);
        GUI.color = Color.white;

        EditorGUI.indentLevel++;
        my_target.show_armor_UI = EditorGUILayout.Toggle("Show armor UI", my_target.show_armor_UI);

        for (int i = 0; i < my_target.gem_length; i++)
        {
            thisCharacter.armor[i] = (Character.gemColorArmor)EditorGUILayout.EnumPopup("armor vs gem " + i, thisCharacter.armor[i]);
        }

        EditorGUI.indentLevel--;
        


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

  


}
