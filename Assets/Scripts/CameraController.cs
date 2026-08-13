using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform targetTransform;

    [Header("Movement")]
    [SerializeField] private float movementSmoothing;

    private Vector3 _offset;
    private Vector3 _currentVelocity;

    private void Start()
    {
        _offset = transform.position - targetTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = targetTransform.position + _offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, movementSmoothing);
    }
}
