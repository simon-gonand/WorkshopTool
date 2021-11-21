using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapProfileWindow : EditorWindow
{
    private MapProfile currentProfile;
    private Map currentMap;

    private float marginRatio = 0.05f;  
    private float heightSpace = 5.0f;
    private float widthSpace = 5.0f;

    private bool isVoid;
    private bool isWall;
    private bool isSpawnerEnemies;
    private bool isSpawnerPlayer;
    private bool isBonus;

    private bool isClick;

    public void InitializeWindow(MapProfile profile)
    {
        currentProfile = profile;
        currentProfile.currentCellType = CellType.Void;

        currentMap = Object.FindObjectOfType<Map>() as Map;

        isVoid = true;
        isWall = false;
        isSpawnerEnemies = false;
        isSpawnerPlayer = false;
        isBonus = false;
    }

    void ProcessEvents()
    {
        // Get if the user is pressing the mouse button
        if (Event.current.type == EventType.MouseDown)
            isClick = true;
        else if (Event.current.type == EventType.MouseUp)
            isClick = false;
    }

    private void CheckToggleStates()
    {
        // Create toggles to create different blocks
        if (isVoid && currentProfile.currentCellType != CellType.Void)
        {
            currentProfile.currentCellType = CellType.Void;
            isWall = false;
            isSpawnerEnemies = false;
            isSpawnerPlayer = false;
            isBonus = false;
        }
        if (isWall && currentProfile.currentCellType != CellType.Wall)
        {
            currentProfile.currentCellType = CellType.Wall;
            isVoid = false;
            isSpawnerEnemies = false;
            isSpawnerPlayer = false;
            isBonus = false;
        }

        else if (isSpawnerEnemies && currentProfile.currentCellType != CellType.SpawnerEnemies)
        {
            currentProfile.currentCellType = CellType.SpawnerEnemies;
            isVoid = false;
            isWall = false;
            isSpawnerPlayer = false;
            isBonus = false;
        }

        else if (isSpawnerPlayer && currentProfile.currentCellType != CellType.SpawnerPlayer)
        {
            currentProfile.currentCellType = CellType.SpawnerPlayer;
            isVoid = false;
            isWall = false;
            isSpawnerEnemies = false;
            isBonus = false;
        }

        if (isBonus && currentProfile.currentCellType != CellType.Bonuses)
        {
            currentProfile.currentCellType = CellType.Bonuses;
            isVoid = false;
            isWall = false;
            isSpawnerEnemies = false;
            isSpawnerPlayer = false;
        }
    }

    private void ResetSpawnerPlayerCell()
    {
        // It exists only one player spawn position
        for (int i = 0; i < currentProfile.cells.Length; ++i)
        {
            if (currentProfile.cells[i] == CellType.SpawnerPlayer)
            {
                currentProfile.cells[i] = CellType.Void;
                int x = i / currentProfile.height;
                int y = i - (x * currentProfile.height);
                UpdateScene(x, y, CellType.Void, currentProfile);
            }
        }
    }

    private static Vector3 GetPositionOnScene(int i, int j, MapProfile currentProfile)
    {
        // Return the position from the grid in the scene
        int originX = i - currentProfile.width / 2;
        int originY = -j + currentProfile.height / 2;
        return new Vector3(originX + 0.5f, 0.0f, originY - 0.5f);
    }

    public static void UpdateScene(int i, int j, CellType cellType, MapProfile currentProfile)
    {
        // Get scene position of the object that the user wants to update
        Vector3 position = GetPositionOnScene(i, j, currentProfile);
        GameObject map = (Object.FindObjectOfType<Map>() as Map).gameObject;
        Transform[] environments = map.GetComponentsInChildren<Transform>();

        // Check in the cells the ones that is in the current position
        bool canInstantiate = true;
        foreach (Transform t in environments)
        {
            if (Mathf.Approximately(t.position.x, position.x) && Mathf.Approximately(t.position.z, position.z))
            {
                // If the cell is different of the user is selecting void => destroy
                if (cellType != currentProfile.cells[j * currentProfile.height + i] 
                    || cellType == CellType.Void)
                {
                    DestroyImmediate(t.gameObject);
                }
                else
                    canInstantiate = false;
            }
        }

        GameObject obj = null;
        if (canInstantiate)
        {
            // According the cell type that has been selecting by the user create the correct object
            switch (cellType)
            {
                case CellType.Wall:
                    obj = PrefabUtility.InstantiatePrefab(currentProfile.wallPrefab) as GameObject;
                    position.y += obj.transform.localScale.y / 2;
                    break;
                case CellType.SpawnerEnemies:
                    obj = PrefabUtility.InstantiatePrefab(currentProfile.enemySpawnerPrefab) as GameObject;
                    position.y -= obj.transform.localScale.y - 0.05f;
                    break;
                case CellType.SpawnerPlayer:
                    obj = new GameObject("PlayerSpawnPoint");
                    obj.tag = "Player";
                    break;
                case CellType.Bonuses:
                    obj = PrefabUtility.InstantiatePrefab(currentProfile.bonusesPrefab[0]) as GameObject;
                    position.y += obj.transform.localScale.y / 2 + 0.25f;
                    break;
                default:
                    return;
            }
            if (obj == null) return;
            obj.transform.position = new Vector3(position.x, position.y, position.z);
            obj.transform.SetParent(map.transform);
        }
    }

    private void OnGUI()
    {
        ProcessEvents();

        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        isVoid = GUILayout.Toggle(isVoid, "Void");
        isWall = GUILayout.Toggle(isWall, "Wall");
        isSpawnerEnemies = GUILayout.Toggle(isSpawnerEnemies, "Spawner Enemies");
        isSpawnerPlayer = GUILayout.Toggle(isSpawnerPlayer, "Spawner Player");
        isBonus = GUILayout.Toggle(isBonus, "Bonus");
        if (EditorGUI.EndChangeCheck())
        {
            // To have only one toggle checked
            CheckToggleStates();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        Rect nextRect = EditorGUILayout.GetControlRect();
        if (nextRect.y == 0) return;

        // Define cell width to be responsive
        float sumSpaceWidth = widthSpace * currentProfile.width;
        float totalWidth = position.width - sumSpaceWidth;  
        float gridWidth = totalWidth * (1f - 2f * marginRatio);
        float cellWidth = gridWidth / currentProfile.width;

        // Define cell height to be responsive
        float sumSpaceHeight = currentProfile.height * heightSpace;
        float totalHeight = position.height - sumSpaceHeight - nextRect.y;
        float gridHeight = totalHeight * (1f - 2f * marginRatio);
        float cellHeight = gridHeight / currentProfile.height;

        Rect gridMap = new Rect(totalWidth * marginRatio, nextRect.y +  gridHeight * marginRatio, gridWidth, gridHeight);

        // Draw grid
        float curY = gridMap.y;
        for (int i = 0; i < currentProfile.width; ++i)
        {
            float curX = gridMap.x;
            for (int j = 0; j < currentProfile.height; ++j)
            {
                Rect cell = new Rect(curX, curY, cellWidth, cellHeight);
                int index = j * currentProfile.height + i;
                // If user is clicking on one cell then he is updating the grid and the scene
                if (isClick && cell.Contains(Event.current.mousePosition))
                {
                    if (currentProfile.currentCellType == CellType.SpawnerPlayer && 
                        currentProfile.cells[index] != CellType.SpawnerPlayer)
                        ResetSpawnerPlayerCell();
                    currentProfile.cells[index] = currentProfile.currentCellType;
                    if(currentMap.mapProfile == currentProfile)
                        UpdateScene(j, i, currentProfile.cells[index], currentProfile);
                }
                EditorGUI.DrawRect(cell, currentProfile.cellTypeColors[(int)currentProfile.cells[index]]);
                curX += cellWidth + widthSpace;
            }
            curY += cellHeight + heightSpace;
        }
        Repaint();
        EditorUtility.SetDirty(currentProfile);
    }
}
