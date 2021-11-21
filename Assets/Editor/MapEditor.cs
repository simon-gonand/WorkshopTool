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
        // Rebuild the map when the profile of the map has been change
        MapProfile mapProfile = profile.objectReferenceValue as MapProfile;
        Transform[] environment = mapScript.gameObject.GetComponentsInChildren<Transform>();

        // Destroy all objects on the map
        foreach (Transform t in environment)
        {
            if (t.GetComponent<Map>()) continue;
            DestroyImmediate(t.gameObject);
        }

        // Recreate objects on the map from the profile
        for (int i = 0; i < mapProfile.cells.Length; ++i)
        {
            // Get x and y values with the map
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

        // Open a window to create a new map profile
        if (GUILayout.Button("Create a Profile"))
        {
            CreateMapProfileWindow window = EditorWindow.GetWindow(typeof(CreateMapProfileWindow)) as CreateMapProfileWindow;
            window.InitializeWindow(mapScript, serializedObject);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
