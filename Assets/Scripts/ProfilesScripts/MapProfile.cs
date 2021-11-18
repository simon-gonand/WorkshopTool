using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Void,
    Wall,
    SpawnerEnemies,
    SpawnerPlayer,
    Bonuses
}

public class MapProfile : ScriptableObject
{
    public int width;
    public int height;

    public GameObject wallPrefab;
    public GameObject enemySpawnerPrefab;
    public GameObject[] bonusesPrefab;

#if UNITY_EDITOR
    public CellType currentCellType;
    public Color[] cellTypeColors;
#endif
    public CellType[] cells;
}
