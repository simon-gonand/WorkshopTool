using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public Waypoint start;
    public Waypoint end;

    public List<Vector3> pathPoints = new List<Vector3>();

    public bool Equals(Waypoint start, Waypoint end)
    {
        return this.start == start && this.end == end;
    }
}
