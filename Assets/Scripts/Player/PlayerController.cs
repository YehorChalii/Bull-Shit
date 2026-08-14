using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float moveAccelerationTime;
    [SerializeField] private float moveDecelerationTime;

    [Header("Rotation")]
    [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float rotationAccelerationTime;

    [Header("Grounding")]
    [SerializeField] private float groundStickSpeed;

    private Vector2 _inputVector;

    private CharacterController _characterController;

    private float _currentMovementSpeed;
    private float _currentRotationSpeed;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _characterController = GetComponent<CharacterController>();
    }

    public void UpdateMovementInputVector(Vector2 inputVector)
    {
        _inputVector = inputVector;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        UpdateCurrentMovementSpeed(deltaTime);
        UpdateCurrentRotationSpeed(deltaTime);

        ApplyMovement(deltaTime);
        ApplyRotation(deltaTime);
    }

    private void UpdateCurrentMovementSpeed(float deltaTime)
    {
        float targetSpeed = _inputVector.y * maxMovementSpeed;

        bool isSpeedingUp = Mathf.Abs(targetSpeed) > Mathf.Abs(_currentMovementSpeed) &&
                             Mathf.Sign(targetSpeed) == Mathf.Sign(_currentMovementSpeed == 0 ? targetSpeed : _currentMovementSpeed);

        float duration = (targetSpeed == 0f || !isSpeedingUp) ? moveDecelerationTime : moveAccelerationTime;

        _currentMovementSpeed = ApproachTarget(_currentMovementSpeed, targetSpeed, duration, deltaTime);
    }

    private void UpdateCurrentRotationSpeed(float deltaTime)
    {
        if (Mathf.Approximately(_inputVector.x, 0f))
        {
            _currentRotationSpeed = 0f;
            return;
        }

        float targetRotationSpeed = _inputVector.x * maxRotationSpeed;
        _currentRotationSpeed = ApproachTarget(_currentRotationSpeed, targetRotationSpeed, rotationAccelerationTime, deltaTime);
    }

    private void ApplyMovement(float deltaTime)
    {
        Vector3 movement = transform.forward * (_currentMovementSpeed * deltaTime);
        movement += Vector3.down * (groundStickSpeed * deltaTime);
        _characterController.Move(movement);
    }

    private void ApplyRotation(float deltaTime)
    {
        if (_currentRotationSpeed != 0f)
        {
            transform.Rotate(Vector3.up, _currentRotationSpeed * deltaTime);
        }
    }

    private float ApproachTarget(float current, float target, float duration, float deltaTime)
    {
        if (duration <= 0f)
        {
            return target;
        }

        float lerpFactor = 1f - Mathf.Exp(-deltaTime / duration);
        float result = Mathf.Lerp(current, target, lerpFactor);

        if (Mathf.Abs(result - target) < 0.01f)
        {
            result = target;
        }

        return result;
    }
}