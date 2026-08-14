using System.Collections.Generic;
using UnityEngine;

public class RobotWaypointPath : MonoBehaviour
{
    private List<Transform> _waypoints = new List<Transform>();

    public IReadOnlyList<Transform> Waypoints => _waypoints;

    private void Awake()
    {
        InitializeWaypoints();
    }

    private void InitializeWaypoints()
    {
        _waypoints.Clear();
        foreach (Transform child in transform)
        {
            _waypoints.Add(child);
        }
    }
}
