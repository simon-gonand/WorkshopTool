using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnerPoolManagerWindow : EditorWindow
{
    SerializedProperty poolSize;
    SerializedProperty enemyPrefab;
    SerializedProperty enemies;
    SerializedProperty enemySpawnRate;

    SpawnerPoolManager poolManagerScript;

    SerializedObject serializedObject;

    public void InitializeWindow(SerializedObject serializedObject, SpawnerPoolManager script)
    {
        this.serializedObject = serializedObject;

        poolSize = serializedObject.FindProperty("poolSize");
        enemyPrefab = serializedObject.FindProperty("enemyPrefab");
        enemies = serializedObject.FindProperty("enemies");
        enemySpawnRate = serializedObject.FindProperty("enemySpawnRate");

        poolManagerScript = script;
    }

    private void RebuildPool()
    {
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        serializedObject.Update();

        // Destoy objects that were in the pool
        while (enemies.arraySize > 0)
        {
            SerializedProperty cur = enemies.GetArrayElementAtIndex(0);
            Object ob = cur.objectReferenceValue;
            if (ob == null) enemies.DeleteArrayElementAtIndex(0);
            else
            {
                DestroyImmediate((ob as EthanBehaviour).gameObject);
                enemies.DeleteArrayElementAtIndex(0);
            }
        }

        // Recreate pool
        for (int i = 0; i < poolSize.intValue; ++i)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(enemyPrefab.objectReferenceValue as GameObject) as GameObject;
            Transform t = go.transform;
            t.SetParent(poolManagerScript.transform);
            t.localPosition = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
            Vector3 pos = new Vector3(poolManagerScript.transform.position.x, t.position.y + t.lossyScale.y, poolManagerScript.transform.position.z);
            t.position = pos;

            enemies.InsertArrayElementAtIndex(enemies.arraySize);
            enemies.GetArrayElementAtIndex(enemies.arraySize - 1).objectReferenceValue = go.GetComponent<EthanBehaviour>();
        }
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    public void OnGUI()
    {
        serializedObject.Update();

        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(poolSize);
        if (EditorGUI.EndChangeCheck())
        {
            if (poolSize.intValue < 0) poolSize.intValue = 0;
        }

        if (GUILayout.Button("Rebuild Pool", EditorStyles.miniButton))
            RebuildPool();
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(enemyPrefab);
        EditorGUILayout.PropertyField(enemySpawnRate);
        serializedObject.ApplyModifiedProperties();
    }
}
