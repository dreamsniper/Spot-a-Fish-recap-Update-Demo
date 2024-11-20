using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor {

    public override void OnInspectorGUI()
    {

        AudioManager my_target = (AudioManager)target;



        if (DrawDefaultInspector()) //show original inspector GUI 
        {
       

        }


        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "AudioManagerEditor");

        if (my_target.bonus_sfx.Length != Enum.GetNames(typeof(Bonus)).Length || my_target.bonus_sfx.Length == 0)
            my_target.bonus_sfx = new AudioClip[Enum.GetNames(typeof(Bonus)).Length];

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bonus:");
        EditorGUI.indentLevel++;
        for (int i = 1; i < Enum.GetNames(typeof(Bonus)).Length; i++)
        {
            my_target.bonus_sfx[i] = EditorGUILayout.ObjectField(Enum.GetName(typeof(Bonus), i).ToString() + " sfx", my_target.bonus_sfx[i], typeof(AudioClip), false) as AudioClip;
        }
        EditorGUI.indentLevel--;


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
