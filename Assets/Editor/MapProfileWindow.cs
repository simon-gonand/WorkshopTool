using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapProfileWindow : EditorWindow
{
    private MapProfile currentProfile;

    private float marginRatio = 0.05f;  
    private float heightSpace = 5.0f;
    private float widthSpace = 5.0f;
    private float cellHeight = 30.0f;

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

        isVoid = true;
        isWall = false;
        isSpawnerEnemies = false;
        isSpawnerPlayer = false;
        isBonus = false;
}

    void ProcessEvents()
    {
        if (Event.current.type == EventType.MouseDown)
            isClick = true;
        else if (Event.current.type == EventType.MouseUp)
            isClick = false;
    }

    private void CheckToggleStates()
    {
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
            CheckToggleStates();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        Rect nextRect = EditorGUILayout.GetControlRect();

        float totalWidth = EditorGUIUtility.currentViewWidth;
        float sumSpaceWidth = widthSpace * currentProfile.width;
        float gridWidth = totalWidth * (1f - 2f * marginRatio);
        float cellWidth = gridWidth / currentProfile.width;
        float sumSpaceHeight = currentProfile.height * heightSpace + sumSpaceWidth;
        float totalHeight = currentProfile.height * cellHeight + sumSpaceHeight;

        Rect gridMap = new Rect(totalWidth * marginRatio, nextRect.y, gridWidth, totalHeight);

        float curY = gridMap.y;
        for (int i = 0; i < currentProfile.width; ++i)
        {
            float curX = gridMap.x;
            for (int j = 0; j < currentProfile.height; ++j)
            {
                Rect cell = new Rect(curX, curY, cellWidth, cellHeight);
                int index = j * currentProfile.height + i;
                if (isClick && cell.Contains(Event.current.mousePosition))
                {
                    Debug.Log("Cell at " + curY + " finish at " + (curY + cellHeight));
                    Debug.Log("MousePos " + Event.current.mousePosition.y);
                    currentProfile.cells[index] = currentProfile.currentCellType;
                }
                EditorGUI.DrawRect(cell, currentProfile.cellTypeColors[(int)currentProfile.cells[index]]);
                curX += cellWidth + widthSpace;
            }
            curY += cellHeight + heightSpace;
        }
        EditorUtility.SetDirty(currentProfile);
    }
}
