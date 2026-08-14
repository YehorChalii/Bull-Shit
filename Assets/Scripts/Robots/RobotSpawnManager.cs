using System.Collections.Generic;
using UnityEngine;

public class RobotSpawnManager : MonoBehaviour
{
    [SerializeField] private RobotController robot;
    [SerializeField] private List<RobotWaypointPath> robotWaypointPaths = new List<RobotWaypointPath>();

    private void Start()
    {
        SpawnRobot();
    }

    private void SpawnRobot()
    {
        if (robotWaypointPaths.Count == 0)
        {
            return;
        }

        RobotWaypointPath randomPath = robotWaypointPaths[Random.Range(0, robotWaypointPaths.Count)];
        Vector3 spawnPosition = randomPath.transform.position;
        RobotController spawnedRobot = Instantiate(robot, spawnPosition, robot.transform.rotation);
        spawnedRobot.SetWaypoints(randomPath.Waypoints);
    }
}
