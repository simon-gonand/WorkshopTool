using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    SerializedProperty waypointsList;
    SerializedProperty linksList;

    ReorderableList waypointsRList;
    ReorderableList linksRList;

    Waypoint selectedWaypoint;
    int selectedWaypointIndex;
    Link selectedLink;
    int selectedLinkIndex;

    Path pathScript;
        
    private void OnEnable()
    {
        // Initialize SerializeProperty and target script
        waypointsList = serializedObject.FindProperty("waypoints");
        linksList = serializedObject.FindProperty("links");
        pathScript = target as Path;

        // Waypoint ReorderableList
        waypointsRList = new ReorderableList(serializedObject, waypointsList);
        waypointsRList.drawHeaderCallback += HeaderWaypointCallback;
        waypointsRList.onSelectCallback += SelectWaypointCallback;
        waypointsRList.drawElementCallback += ElementWaypointCallback;
        waypointsRList.onAddCallback += AddWaypointCallback;
        waypointsRList.onRemoveCallback += RemoveWaypointCallback;
        waypointsRList.draggable = false;

        // Link ReorderableList
        linksRList = new ReorderableList(serializedObject, linksList);
        linksRList.drawHeaderCallback += HeaderLinkCallback;
        linksRList.onSelectCallback += SelectLinkCallback;
        linksRList.drawElementCallback += ElementLinkCallback;
        linksRList.draggable = false;
        linksRList.displayAdd = false;
        linksRList.displayRemove = false;
    }

    #region waypoints reorderable list
    private void HeaderWaypointCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Waypoints");
    }

    private void SelectWaypointCallback(ReorderableList rList)
    {
        // Define which waypoint to display on screen
        SerializedProperty sp = waypointsList.GetArrayElementAtIndex(rList.index);
        selectedWaypoint = sp.objectReferenceValue as Waypoint;
        selectedWaypointIndex = rList.index;
        selectedLink = null;
    }

    private void ElementWaypointCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.y += 2;
        Waypoint waypoint = (waypointsList.GetArrayElementAtIndex(index).objectReferenceValue as Waypoint);

        // Divide property drawer into three part (label, adding button and position values)
        EditorGUI.BeginChangeCheck();
        Rect leftRect = new Rect(rect.x, rect.y, rect.width / 2, rect.height);
        Rect labelRect = new Rect(rect.x, rect.y, leftRect.width - leftRect.width / 5, rect.height);
        EditorGUI.LabelField(labelRect, "Waypoint " + index);

        Rect buttonRect = new Rect(labelRect.x + labelRect.width, rect.y, leftRect.width / 5, rect.height);
        if (GUI.Button(buttonRect, "+"))
        {
            GenericMenu addMenu = new GenericMenu();

            // Add a waypoint either above the selected point or below
            addMenu.AddItem(new GUIContent("Add waypoint above"), false, CreateWaypointAndLinks, index);
            addMenu.AddItem(new GUIContent("Add waypoint below"), false, CreateWaypointAndLinks, index + 1);

            addMenu.ShowAsContext();
        }

        // Display position
        Rect rightRect = new Rect(leftRect.x + leftRect.width, rect.y, rect.width / 2, rect.height);
        waypoint.transform.position = EditorGUI.Vector3Field(rightRect, GUIContent.none, waypoint.transform.position);
        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(waypoint.gameObject);
    }

    private void AddWaypointCallback(ReorderableList rlist)
    {
        // Create waypoint at the end of the list
        CreateWaypoint(waypointsList.arraySize);
        if (waypointsList.arraySize > 1)
            CreateLinks(); // Create links between waypoints if necessary
        // Select the created waypoint to handle it 
        waypointsRList.index = waypointsList.arraySize - 1;
        waypointsRList.onSelectCallback(waypointsRList);
    }

    private void RemoveWaypointCallback(ReorderableList rlist)
    {
        // Unselect waypoint
        selectedWaypoint = null;
        // if there is no link just delete waypoin
        if (linksList.arraySize == 0)
        {
            waypointsList.DeleteArrayElementAtIndex(rlist.index);
            return;
        }
        // If it's the first waypoint of the path just remove first link + first waypoint
        if (rlist.index == 0)
        {
            linksList.DeleteArrayElementAtIndex(0);
            waypointsList.DeleteArrayElementAtIndex(rlist.index);
            return;
        }
        // If it's the last waypoint of the path just remove last link + last waypoint
        if (rlist.index == linksList.arraySize)
        {
            linksList.DeleteArrayElementAtIndex(linksList.arraySize - 1);
            waypointsList.DeleteArrayElementAtIndex(rlist.index);
            return;
        }

        // Else remove the previous and the next link and create a new one between the previous and the next waypoint
        linksList.DeleteArrayElementAtIndex(rlist.index - 1);
        linksList.DeleteArrayElementAtIndex(rlist.index - 1);
        Waypoint start = waypointsList.GetArrayElementAtIndex(rlist.index - 1).objectReferenceValue as Waypoint;
        Waypoint end = waypointsList.GetArrayElementAtIndex(rlist.index + 1).objectReferenceValue as Waypoint;
        CreateLink(start, end, rlist.index - 1);
        waypointsList.DeleteArrayElementAtIndex(rlist.index);
    }
    #endregion

    #region links reorderable list
    private void HeaderLinkCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Links");
    }

    private void SelectLinkCallback(ReorderableList rList)
    {
        // Define which link to display on screen
        SerializedProperty sp = linksList.GetArrayElementAtIndex(rList.index);
        selectedLink = sp.objectReferenceValue as Link;
        selectedLinkIndex = rList.index;
        selectedWaypoint = null;
    }
    private void ElementLinkCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.y += 2;
        EditorGUI.LabelField(rect, index + " - " + (index + 1));
    }
    #endregion

    private void CreateWaypointAndLinks(object index)
    {
        // Create waypoints at a certain index and manage links according to the new order
        serializedObject.Update();
        int i = (int)index;

        //Create the waypoint
        CreateWaypoint(i);
        serializedObject.ApplyModifiedPropertiesWithoutUndo();

        // If the new waypoint is not the first one or the last one of the path and there is more than one waypoint on the path
        if (i > 0 && i < waypointsList.arraySize - 1 && linksList.arraySize > 0)
        {
            // Delete link between the previous waypoint and the next one to insert the new one
            linksList.DeleteArrayElementAtIndex(i - 1);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        // If the new waypoint is not the first one of the path
        if (i > 0)
        {
            // Create a link between the previous waypoint and the created one
            Waypoint start = waypointsList.GetArrayElementAtIndex(i - 1).objectReferenceValue as Waypoint;
            Waypoint end = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;
            // If there is no link in the array do not create it at -1 but at 0
            if (linksList.arraySize == 0)
            {
                Link link = CreateLink(start, end, 0);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Link link = CreateLink(start, end, i - 1);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        // If the new waypoint is not the last one of the path and there is more waypoint than link
        if (i  < waypointsList.arraySize && waypointsList.arraySize > linksList.arraySize + 1)
        {
            Waypoint start = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;
            Waypoint end = waypointsList.GetArrayElementAtIndex(i + 1).objectReferenceValue as Waypoint;

            // If there is no link and the index is 1, avoid to create at index 1
            if (linksList.arraySize == 0)
            {
                Link link = CreateLink(start, end, 0);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Link link = CreateLink(start, end, i);
                CalculateLink(link);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateWaypoint(int index)
    {
        // Create a gameObject to put the waypoint
        GameObject obj = new GameObject();
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(pathScript.transform);
        obj.hideFlags = HideFlags.HideInHierarchy;
        Waypoint wp = obj.AddComponent<Waypoint>();
        
        // Update list
        waypointsList.InsertArrayElementAtIndex(index);
        waypointsList.GetArrayElementAtIndex(index).objectReferenceValue = wp;
        EditorUtility.SetDirty(obj);
        Undo.RegisterCreatedObjectUndo(obj, "Create waypoint");
    }

    // Calculate link path between waypoints
    private void CalculateLinks()
    {
        if (linksList.arraySize <= 0) return;

        for (int i = 0; i < linksList.arraySize; ++i)
        {
            Link link = linksList.GetArrayElementAtIndex(i).objectReferenceValue as Link;
            CalculateLink(link);
        }
    }

    private void CalculateLink(Link link)
    {
        // Calculate path between waypoint using the navmesh
        NavMeshPath linkPath = new NavMeshPath();
        if (NavMesh.CalculatePath(link.start.transform.position, link.end.transform.position, NavMesh.AllAreas, linkPath))
        {
            // Disable previous points that were calculated
            if (link.pathPoints.Count > 0)
                link.pathPoints.Clear();
        }
        // Get all corners from the navMesh
        for (int j = 0; j < linkPath.corners.Length; ++j)
        {
            link.pathPoints.Add(linkPath.corners[j]);
        }
    }

    private Link CreateLink(Waypoint start, Waypoint end, int index)
    {
        // Create a game object with the link inside
        GameObject go = new GameObject("Link");
        go.transform.SetParent(pathScript.transform);
        go.hideFlags = HideFlags.HideInHierarchy;
        Link link = go.AddComponent<Link>();
        link.start = start;
        link.end = end;
        
        // Update list
        linksList.InsertArrayElementAtIndex(index);
        linksList.GetArrayElementAtIndex(index).objectReferenceValue = link;
        return link;
    }

    // Create links according to waypoints
    private void CreateLinks()
    {
        for (int i = 1; i < waypointsList.arraySize; ++i)
        {
            Waypoint start = waypointsList.GetArrayElementAtIndex(i - 1).objectReferenceValue as Waypoint;
            Waypoint end = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;

            // Avoid to duplicate links
            bool linkAlreadyExist = false;
            for (int j = 0; j < linksList.arraySize; ++j)
            {
                Link l = linksList.GetArrayElementAtIndex(j).objectReferenceValue as Link;
                if (l.Equals(start, end))
                {
                    linkAlreadyExist = true;
                    break;
                }
            }
            if (linkAlreadyExist) continue;

            CreateLink(start, end, linksList.arraySize);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        waypointsRList.DoLayoutList();
        linksRList.DoLayoutList();
        
        if (GUILayout.Button("Calculate Path"))
            CalculateLinks();
        if (GUILayout.Button("Calculate Selected Link"))
        {
            if (!selectedLink)
            {
                Debug.LogWarning("No link has been selected");
                return;
            }
            CalculateLink(selectedLink);
        }
        
        if (GUILayout.Button("Erase all"))
        {
            while(waypointsList.arraySize !=0 || linksList.arraySize != 0)
            {
                waypointsList.DeleteArrayElementAtIndex(0);
                if (linksList.arraySize == 0) continue;
                linksList.DeleteArrayElementAtIndex(0);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        // Remove handles of unity on path object
        Tools.current = Tool.None;

        // Draw disc to show waypoints
        for (int i = 0; i < waypointsList.arraySize; ++i)
        {
            Waypoint waypoint = waypointsList.GetArrayElementAtIndex(i).objectReferenceValue as Waypoint;
            Handles.DrawSolidDisc(waypoint.transform.position, Vector3.up, 0.3f);
        }

        // Draw line between waypoints once the links are calculated
        Handles.color = Color.red;
        for (int i = 0; i < linksList.arraySize; ++i)
        {
            Link link = linksList.GetArrayElementAtIndex(i).objectReferenceValue as Link;
            for (int j = 1; j < link.pathPoints.Count; ++j)
                Handles.DrawLine(link.pathPoints[j - 1], link.pathPoints[j]);
        }

        // If a waypoint has been selected => show handles to move it
        if (selectedWaypoint) {
            Undo.RecordObject(selectedWaypoint.transform, "Move Waypoints");
            selectedWaypoint.transform.position = Handles.PositionHandle(
                selectedWaypoint.transform.position,
                selectedWaypoint.transform.rotation);
            EditorUtility.SetDirty(selectedWaypoint.transform);

            // Update links when selected waypoint is moving
            if (linksList.arraySize > 0)
            {
                if (selectedWaypointIndex < linksList.arraySize)
                {
                    Link link = linksList.GetArrayElementAtIndex(selectedWaypointIndex).objectReferenceValue as Link;
                    if (link.pathPoints.Count == 0) return;
                    link.pathPoints[0] = selectedWaypoint.transform.position;
                }
                if (selectedWaypointIndex > 0)
                {
                    Link link = linksList.GetArrayElementAtIndex(selectedWaypointIndex - 1).objectReferenceValue as Link;
                    if (link.pathPoints.Count == 0) return;
                    link.pathPoints[link.pathPoints.Count - 1] = selectedWaypoint.transform.position;
                }
            }
        }

        // If a link has been selected and has been calculated
        if (selectedLink && selectedLink.pathPoints.Count > 0)
        {
            // Put handles on every corners of the path in the link
            for (int i = 0; i < selectedLink.pathPoints.Count; ++i)
            {
                Undo.RecordObject(selectedLink, "Move Path points");
                selectedLink.pathPoints[i] = Handles.PositionHandle(
                    selectedLink.pathPoints[i],
                    selectedLink.transform.rotation);
                EditorUtility.SetDirty(selectedLink);
            }
            
            // Update link points and waypoint if there is a movement
            selectedLink.start.transform.position = selectedLink.pathPoints[0];
            if (selectedLinkIndex > 0)
            {
                Link previousLink = linksList.GetArrayElementAtIndex(selectedLinkIndex - 1).objectReferenceValue as Link;
                previousLink.pathPoints[previousLink.pathPoints.Count - 1] = selectedLink.pathPoints[0];
                previousLink.end.transform.position = selectedLink.pathPoints[0];
            }

            selectedLink.end.transform.position = selectedLink.pathPoints[selectedLink.pathPoints.Count - 1];
            if (selectedLinkIndex < linksList.arraySize - 1)
            {
                Link nextLink = linksList.GetArrayElementAtIndex(selectedLinkIndex + 1).objectReferenceValue as Link;
                nextLink.pathPoints[0] = selectedLink.end.transform.position;
                nextLink.start.transform.position = nextLink.pathPoints[0];
            }
        }
    }
}
