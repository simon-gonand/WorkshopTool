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

        // Let the user select a folder
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
            // Create the new map profile and initialize values
            MapProfile newProfile = ScriptableObject.CreateInstance<MapProfile>();
            newProfile.width = (int)mapScript.transform.lossyScale.x;
            newProfile.height = (int)mapScript.transform.lossyScale.z;
            newProfile.cells = new CellType[newProfile.width * newProfile.height];
            newProfile.cellTypeColors = new Color[5] { Color.white, Color.black, Color.red, Color.cyan, Color.yellow };

            // Set as the current selected profile for the map
            profile.objectReferenceValue = newProfile;

            if (profileName == "")
            {
                Debug.LogWarning("Profile does not have a name");
            }
            else
            {
                // Save it into the selected path
                if (path == "")
                    AssetDatabase.CreateAsset(newProfile, "Assets/" + profileName + ".asset");
                else
                    AssetDatabase.CreateAsset(newProfile, "Assets/" + path + "/" + profileName + ".asset");

                Transform[] environment = mapScript.GetComponentsInChildren<Transform>();
                foreach (Transform t in environment)
                {
                    if (t.GetComponent<Map>()) continue;
                    DestroyImmediate(t.gameObject);
                }
            }
        }
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }
}
