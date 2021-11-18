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

    private void RebuildMap()
    {
        MapProfile mapProfile = profile.objectReferenceValue as MapProfile;
        Transform[] environment = mapScript.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in environment)
        {
            if (t.GetComponent<Map>()) continue;
            DestroyImmediate(t.gameObject);
        }
        for (int i = 0; i < mapProfile.cells.Length; ++i)
        {
            int x = i / mapProfile.height;
            int y = i - (x * mapProfile.height);
            MapProfileWindow.UpdateScene(x, y, mapProfile.cells[i], mapProfile);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(profile);
        if (EditorGUI.EndChangeCheck())
        {
            RebuildMap();
        }

        if (GUILayout.Button("Create a Profile"))
        {
            CreateMapProfileWindow window = EditorWindow.GetWindow(typeof(CreateMapProfileWindow)) as CreateMapProfileWindow;
            window.InitializeWindow(mapScript, serializedObject);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
