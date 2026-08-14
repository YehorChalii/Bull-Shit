using System.Collections.Generic;
using UnityEngine;

public class RobotMovementController : MonoBehaviour
{
    private enum MovementState
    {
        Idle,
        RotatingToWaypoint,
        MovingToWaypoint,
        FacingTarget
    }

    [Header("Movement")]
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float movementSmoothing;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed;

    [Header("State Transitions")]
    [SerializeField] private float stateTransitionDelay;

    private IReadOnlyList<Transform> _waypoints;
    private int _currentWaypointIndex;

    private Vector3 _currentVelocity;

    private MovementState _movementState;

    private float _transitionTimer;
    private bool _isTransitioning;
    private MovementState _nextMovementState;

    private Transform _faceTarget;

    public bool FinishedPath { get; private set; }

    public void SetWaypoints(IReadOnlyList<Transform> waypoints)
    {
        _waypoints = waypoints;

        ResetPath();
    }

    public void StartFollowing()
    {
        if (!HasValidPath()) return;

        ResetTransition();

        _faceTarget = null;
        _currentVelocity = Vector3.zero;

        _movementState = MovementState.RotatingToWaypoint;
    }

    public void Stop()
    {
        ResetTransition();

        _faceTarget = null;
        _currentVelocity = Vector3.zero;

        _movementState = MovementState.Idle;
    }

    public void FaceTarget(Transform target)
    {
        if (target == null) return;

        ResetTransition();

        _faceTarget = target;
        _currentVelocity = Vector3.zero;

        _movementState = MovementState.FacingTarget;
    }

    public void ResumeFollowing()
    {
        if (!HasValidPath()) return;

        _currentVelocity = Vector3.zero;

        BeginTransition(MovementState.RotatingToWaypoint);
    }

    public void FollowPath(float deltaTime)
    {
        if (FinishedPath) return;

        if (_movementState == MovementState.Idle) return;

        if (_movementState == MovementState.FacingTarget)
        {
            FaceCurrentTarget(deltaTime);

            if (_isTransitioning)
            {
                UpdateTransitionTimer(deltaTime);
            }

            return;
        }

        if (!HasValidPath()) return;

        if (_isTransitioning)
        {
            UpdateTransitionTimer(deltaTime);
            return;
        }

        switch (_movementState)
        {
            case MovementState.RotatingToWaypoint:
                RotateToCurrentWaypoint(deltaTime);
                break;

            case MovementState.MovingToWaypoint:
                MoveToCurrentWaypoint(deltaTime);
                break;
        }
    }

    private void MoveToCurrentWaypoint(float deltaTime)
    {
        Transform waypoint = _waypoints[_currentWaypointIndex];

        Vector3 targetPosition = RobotMovementCalculator.GetFlatTargetPosition(transform, waypoint);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            movementSmoothing,
            maxMovementSpeed,
            deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            _currentWaypointIndex++;

            if (_currentWaypointIndex >= _waypoints.Count)
            {
                FinishedPath = true;
                _currentVelocity = Vector3.zero;
                return;
            }

            BeginTransition(MovementState.RotatingToWaypoint);
        }
    }

    private void RotateToCurrentWaypoint(float deltaTime)
    {
        Transform waypoint = _waypoints[_currentWaypointIndex];

        var (targetRotation, isTooClose) = GetTargetRotation(waypoint);

        if (isTooClose)
        {
            BeginTransition(MovementState.MovingToWaypoint);
            return;
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * deltaTime
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) <= 0.1f)
        {
            transform.rotation = targetRotation;

            BeginTransition(MovementState.MovingToWaypoint);
        }
    }

    private void FaceCurrentTarget(float deltaTime)
    {
        if (_faceTarget == null) return;

        var (targetRotation, isTooClose) = GetTargetRotation(_faceTarget);

        if (isTooClose) return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * deltaTime
        );
    }

    private (Quaternion targetRotation, bool isTooClose) GetTargetRotation(Transform targetTransform)
    {
        Vector3 targetPosition = RobotMovementCalculator.GetFlatTargetPosition(transform, targetTransform);

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            return (transform.rotation, true);
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        return (targetRotation, false);
    }

    private void BeginTransition(MovementState nextState)
    {
        _nextMovementState = nextState;
        _transitionTimer = stateTransitionDelay;
        _isTransitioning = true;
    }

    private void UpdateTransitionTimer(float deltaTime)
    {
        _transitionTimer -= deltaTime;

        if (_transitionTimer > 0f) return;

        _transitionTimer = 0f;
        _isTransitioning = false;

        _movementState = _nextMovementState;

        _faceTarget = null;
    }

    private void ResetPath()
    {
        ResetTransition();

        FinishedPath = false;
        _currentWaypointIndex = 0;

        _currentVelocity = Vector3.zero;
        _faceTarget = null;
    }

    private void ResetTransition()
    {
        _transitionTimer = 0f;
        _isTransitioning = false;
        _nextMovementState = default;
    }

    private bool HasValidPath()
    {
        return _waypoints != null
               && _waypoints.Count > 0
               && _currentWaypointIndex < _waypoints.Count
               && _waypoints[_currentWaypointIndex] != null;
    }

    private void OnDisable()
    {
        ResetTransition();
    }
}