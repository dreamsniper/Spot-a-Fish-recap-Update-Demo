using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(RulesetTemplate))]
public partial class RulesetTemplateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GemsRules();
        EditorGUILayout.Space();
        RandomObstaclesRules();
        EditorGUILayout.Space();
        MainRules();
        EditorGUILayout.Space();
        BonusRules();

    }

}
