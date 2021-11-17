using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    SerializedProperty profile;

    Map mapScript;

    private void OnEnable()
    {
        profile = serializedObject.FindProperty("mapProfile");

        mapScript = target as Map;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(profile);

        if (GUILayout.Button("Create a Profile"))
        {
            CreateMapProfileWindow window = EditorWindow.GetWindow(typeof(CreateMapProfileWindow)) as CreateMapProfileWindow;
            window.InitializeWindow(mapScript, serializedObject);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
