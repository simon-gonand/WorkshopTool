using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(PoolManager))]
public class SpawnerPoolManagerEditor : Editor
{
    PoolManager script;

    ReorderableList spawnersList;
    SpawnerPoolManager[] spawners;

    private void OnEnable()
    {
        script = target as PoolManager;

        spawners = script.GetComponentsInChildren<SpawnerPoolManager>();
        spawnersList = new ReorderableList(spawners, typeof(SpawnerPoolManager));
        spawnersList.drawElementCallback += OnDrawElementCallback;
        spawnersList.drawHeaderCallback += OnDrawHeaderCallback;
        spawnersList.draggable = false;
        spawnersList.displayAdd = false;
        spawnersList.displayRemove = false;
    }

    private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        Rect leftRect = new Rect(rect.x, rect.y, rect.width / 2, rect.height);
        Rect rightRect = new Rect(leftRect.x + leftRect.width, rect.y, rect.width / 2, rect.height);
        GUI.Label(leftRect, "Spawners Pool " + index);
        if (GUI.Button(rightRect, "Edit Pool"))
        {
            SpawnerPoolManagerWindow poolManagerWindow = EditorWindow.GetWindow<SpawnerPoolManagerWindow>();
            SerializedObject spawnerSerialized = new SerializedObject(spawners[index]);
            poolManagerWindow.InitializeWindow(spawnerSerialized, spawners[index]);
        }
    }

    private void OnDrawHeaderCallback(Rect rect)
    {
        GUI.Label(rect, "Spawners Pool");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        spawnersList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
