using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageFile))]
public class StageFile_editor : Editor {

    MapEditorTextures mapEditorTextures;
    float size_button = 50;
    float spacingUp = 215;
    float spacingLeft = 15;

    void OnEnable()
    {
        mapEditorTextures = new MapEditorTextures();
        mapEditorTextures.LoadTextures();
    }

    public override void OnInspectorGUI()
    {

        StageFile my_target = (StageFile)target;
        EditorGUI.BeginChangeCheck();
        Undo.RecordObject(my_target, "MapEditor");

        bool showMap = false;
        if (my_target.map != null)
        {
            if (my_target.width > 0 && my_target.height > 0)
            showMap = true;
        }

        if (showMap)
        {
            my_target.rules = EditorGUILayout.ObjectField("Rules", my_target.rules, typeof(RulesetTemplate), false) as RulesetTemplate;
            my_target.theme = EditorGUILayout.ObjectField("Theme", my_target.theme, typeof(ThemeTemplate), false) as ThemeTemplate;
            my_target.background = EditorGUILayout.ObjectField("Background", my_target.background, typeof(Sprite), false) as Sprite;
            my_target.camera = EditorGUILayout.ObjectField("Camera", my_target.camera, typeof(CameraTemplate), false) as CameraTemplate;

            EditorGUILayout.LabelField("Map: " + my_target.width.ToString() + " x " + my_target.height.ToString());
            if (GUILayout.Button("Edit"))
                OpenEditorWindow(my_target);

            for (int x = 0; x < my_target.width; x++)
            {
                for (int y = 0; y < my_target.height; y++)
                {
                    Show_tile(x, y);
                    
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("No map!");
            if (GUILayout.Button("Create"))
                OpenEditorWindow(my_target);
        }


        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(my_target);
    }

    void OpenEditorWindow(StageFile thisMap)
    {
        MapEditorWindow.MyInit(thisMap);
    }

    void Show_tile(int x, int y)
    {
        StageFile my_target = (StageFile)target;
        Rect tempRect = new Rect(x * size_button + spacingLeft, y * size_button + spacingUp, size_button, size_button);

        mapEditorTextures.ShowTextures(tempRect, my_target.GetTile(x,y));

    }
}
