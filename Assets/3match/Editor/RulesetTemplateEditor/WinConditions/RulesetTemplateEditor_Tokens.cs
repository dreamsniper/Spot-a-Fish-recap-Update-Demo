using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public partial class RulesetTemplateEditor : Editor
{

    void TakeAllTokensInfo()
    {
        RulesetTemplate my_target = (RulesetTemplate)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "TakeAllTokensInfo");


        if (my_target.win_requirement_selected != Ruleset.win_requirement.take_all_tokens)
            return;

        EditorGUI.indentLevel++;

        my_target.show_token_after_all_tiles_are_destroyed = EditorGUILayout.Toggle("Show token only after all tiles are destroyed", my_target.show_token_after_all_tiles_are_destroyed);

        EditorGUI.indentLevel--;

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }
}
