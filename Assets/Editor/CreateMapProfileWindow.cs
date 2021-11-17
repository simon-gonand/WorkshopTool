using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateMapProfileWindow : EditorWindow
{
    private string path;
    private string profileName;

    private Map mapScript;
    private SerializedObject serializedObject;
    private SerializedProperty profile;
    public void InitializeWindow(Map map, SerializedObject sobj)
    {
        mapScript = map;
        serializedObject = sobj;
        profile = serializedObject.FindProperty("mapProfile");
    }

    private void OnGUI()
    {
        serializedObject.Update();
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

        }
        GUILayout.EndHorizontal();

        profileName = EditorGUILayout.TextField(new GUIContent("Name", "Name of the profile"), profileName);

        if (GUILayout.Button("Create Profile"))
        {
            MapProfile newProfile = new MapProfile();
            newProfile.width = (int)mapScript.transform.lossyScale.x;
            newProfile.height = (int)mapScript.transform.lossyScale.z;
            profile.objectReferenceValue = newProfile;
            if (profileName == "")
            {
                Debug.LogWarning("Profile does not have a name");
            }
            else
            {
                if (path == "")
                    AssetDatabase.CreateAsset(newProfile, "Assets/" + profileName + ".asset");
                else
                    AssetDatabase.CreateAsset(newProfile, "Assets/" + path + "/" + profileName + ".asset");
            }
        }
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
}
