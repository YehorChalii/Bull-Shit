using UnityEngine;

public class RobotContext
{
    public RobotBehaviourController BehaviourController { get; }
    public RobotMovementController MovementController { get; }
    public Transform Transform { get; }

    public RobotContext(RobotBehaviourController behaviourController, RobotMovementController movementController)
    {
        BehaviourController = behaviourController;
        MovementController = movementController;
        Transform = behaviourController.transform;
    }
}