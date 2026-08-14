using UnityEngine;

public static class RobotMovementCalculator
{
    public static Vector3 GetFlatTargetPosition(Transform transform, Transform targetTransform)
    {
        return new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
    }
}
