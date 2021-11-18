using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapProfile))]
public class MapProfileEditor : Editor
{
    SerializedProperty wallPrefab;
    SerializedProperty enemySpawnerPrefab;
    SerializedProperty bonusesPrefab;

    private void OnEnable()
    {
        wallPrefab = serializedObject.FindProperty("wallPrefab");
        enemySpawnerPrefab = serializedObject.FindProperty("enemySpawnerPrefab");
        bonusesPrefab = serializedObject.FindProperty("bonusesPrefab");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(wallPrefab);
        EditorGUILayout.PropertyField(enemySpawnerPrefab);
        EditorGUILayout.PropertyField(bonusesPrefab);

        if (GUILayout.Button("OpenWindow"))
        {
            MapProfileWindow window = EditorWindow.GetWindow(typeof(MapProfileWindow)) as MapProfileWindow;
            window.InitializeWindow(target as MapProfile);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
