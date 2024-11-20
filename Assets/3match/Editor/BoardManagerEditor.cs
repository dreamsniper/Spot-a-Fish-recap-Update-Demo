using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BoardManager))]
public class BoardManagerEditor : Editor {

    public override void OnInspectorGUI()
    {

        BoardManager my_target = (BoardManager)target;



        if (DrawDefaultInspector()) //show original inspector GUI 
        {

        }


        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Load Current Stage"))
        {
            my_target.LoadCurrentStage();
        }


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
