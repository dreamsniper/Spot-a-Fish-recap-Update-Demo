using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TxtConverter))]
public class TxtConverter_editor : Editor {

    public override void OnInspectorGUI()
    {
        TxtConverter my_target = (TxtConverter)target;
        if (GUILayout.Button("Convert"))
            my_target.ConvertAll();

        base.DrawDefaultInspector();
    }
}
