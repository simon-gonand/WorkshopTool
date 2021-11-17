using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    string path;
    string name;

    SerializedProperty profile;

    private void OnEnable()
    {
        profile = serializedObject.FindProperty("mapProfile");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(profile);

        GUILayout.BeginHorizontal();
        path = EditorGUILayout.TextField(new GUIContent("Path", "Where to save profile"), path);
        if (GUILayout.Button("Folder", EditorStyles.miniButton))
        {
            path = EditorUtility.OpenFolderPanel("Path", "Assets", "");
            if (!path.Contains("Assets"))
            {
                path = "";
                Debug.LogWarning("Profile cannot be saved aware from Assets/ folder");
            }
            else
                path = path.Substring(path.LastIndexOf("Assets/") + 7);
            Debug.Log(path);
            
        }
        GUILayout.EndHorizontal();

        name = EditorGUILayout.TextField(new GUIContent("Name", "Name of the profile"), name);

        if (GUILayout.Button("Create Profile"))
        {
            MapProfile newProfile = new MapProfile();
            profile.objectReferenceValue = newProfile;
            if (name == "")
            {
                Debug.LogWarning("Profile does not have a name");
            }
            else
            {
                if (path == "")
                    AssetDatabase.CreateAsset(newProfile, "Assets/" + name + ".asset");
                else
                    AssetDatabase.CreateAsset(newProfile, "Assets/" + path + "/" + name + ".asset");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
