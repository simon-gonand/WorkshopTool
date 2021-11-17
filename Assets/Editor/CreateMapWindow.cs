using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateMapWindow : EditorWindow
{
    int width = 0;
    int height = 0;

    GameObject prefabReference;

    [MenuItem("Tools/Home Tool/Create Map %M")]
    public static void Open()
    {
        GetWindow(typeof(CreateMapWindow));
    }

    private void OnGUI()
    {
        prefabReference = EditorGUILayout.ObjectField("Current Obstacle", prefabReference, typeof(GameObject), false) as GameObject;

        width = EditorGUILayout.IntField(new GUIContent("Width", "Define the width of the level grid"), width);
        height = EditorGUILayout.IntField(new GUIContent("Height", "Define the height of the level grid"), height);
        if (GUILayout.Button("Create Map"))
        {
            if (width <= 0 || height <= 0) return;
            if (prefabReference != null) {
                Map script = Object.FindObjectOfType(typeof(Map)) as Map;
                GameObject go;
                if (script == null) { 
                    go = PrefabUtility.InstantiatePrefab(prefabReference) as GameObject;
                    script = go.AddComponent<Map>();

                    Undo.RegisterCreatedObjectUndo(go, "Create map");
                }
                else
                {
                    go = script.gameObject;
                    go.transform.localScale = Vector3.one;
                }
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                Vector3 newScale = new Vector3(go.transform.lossyScale.x * width, go.transform.lossyScale.y, go.transform.localScale.z * height);
                go.transform.localScale = newScale;
                EditorUtility.SetDirty(go);
            }
        }
    }
}
