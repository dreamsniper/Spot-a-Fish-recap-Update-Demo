using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public partial class RulesetTemplateEditor : Editor
{

    void RandomObstaclesRules()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "RandomObstaclesRules");


        my_target.show_randomObstacles = EditorGUILayout.Foldout(my_target.show_randomObstacles, "Generate Random Obstacles at the Start");
        if (my_target.show_randomObstacles)
        {
            EditorGUI.indentLevel++;

            my_target.randomJunks = EditorGUILayout.IntField("junk", my_target.randomJunks);
            my_target.randomPadlocks = EditorGUILayout.IntField("padlocks", my_target.randomPadlocks);
            my_target.randomIces = EditorGUILayout.IntField("ice", my_target.randomIces);
            my_target.randomBlocks = EditorGUILayout.IntField("blocks", my_target.randomBlocks);
            my_target.randomFallingBlocks = EditorGUILayout.IntField("falling blocks", my_target.randomFallingBlocks);
            my_target.randomGenerativeBlocks = EditorGUILayout.IntField("generative blocks", my_target.randomGenerativeBlocks);

            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);

    }
    
}
