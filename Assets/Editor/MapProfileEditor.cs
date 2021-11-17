using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapProfile))]
public class MapProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("OpenWindow"))
        {
            MapProfileWindow window = EditorWindow.GetWindow(typeof(MapProfileWindow)) as MapProfileWindow;
            window.InitializeWindow(target as MapProfile);
        }  
    }
}
