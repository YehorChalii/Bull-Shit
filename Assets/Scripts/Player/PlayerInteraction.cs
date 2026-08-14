using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<RobotController> _robotControllers = new List<RobotController>();

    public void Hack()
    {
        if (_robotControllers.Count > 0)
        {
            RobotController closestRobotController = _robotControllers[0];
            float minDistance = Vector3.Distance(transform.position, closestRobotController.transform.position);

            foreach (var robotController in _robotControllers)
            {
                float currentDistance = Vector3.Distance(transform.position, robotController.transform.position);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestRobotController = robotController;
                }
            }

            _robotControllers.Remove(closestRobotController);
            closestRobotController.Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot") && other.TryGetComponent<RobotController>(out var robotController))
        {
            robotController.OnPlayerEnter(gameObject);

            _robotControllers.Add(robotController);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Robot") && other.TryGetComponent<RobotController>(out var robotController))
        {
            robotController.OnPlayerExit();

            _robotControllers.Remove(robotController);
        }
    }
}