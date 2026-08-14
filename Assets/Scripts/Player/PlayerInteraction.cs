using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<RobotDetectionController> _robotDetectionControllers = new List<RobotDetectionController>();

    public void Hack()
    {
        if (_robotDetectionControllers.Count > 0)
        {
            RobotDetectionController closestRobotController = _robotDetectionControllers[0];
            float minDistance = Vector3.Distance(transform.position, closestRobotController.transform.position);

            foreach (var robotController in _robotDetectionControllers)
            {
                float currentDistance = Vector3.Distance(transform.position, robotController.transform.position);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestRobotController = robotController;
                }
            }

            _robotDetectionControllers.Remove(closestRobotController);
            closestRobotController.Hack();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot") && other.TryGetComponent<RobotDetectionController>(out var robotController))
        {
            robotController.OnPlayerEnter(gameObject);

            _robotDetectionControllers.Add(robotController);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Robot") && other.TryGetComponent<RobotDetectionController>(out var robotController))
        {
            robotController.OnPlayerExit();

            _robotDetectionControllers.Remove(robotController);
        }
    }
}