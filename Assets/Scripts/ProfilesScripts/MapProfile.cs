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

#if UNITY_EDITOR
    public CellType currentCellType;
#endif
    public Color[] cellTypeColors;
    public CellType[] cells;
}
