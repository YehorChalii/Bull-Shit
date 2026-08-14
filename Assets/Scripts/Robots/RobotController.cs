using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private enum RobotMovementState
    {
        Rotating,
        Moving,
        Delay
    }

    private enum RobotBehaviourState
    {
        Recycling,
        Staring,
        Fleeing
    }

    private RobotBehaviourState _currentBehaviourState;

    [Header("Movement")]
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float movementSmoothing;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed;

    [Header("State Transitions")]
    [SerializeField] private float stateTransitionDelay;

    [Header("UI")]
    [SerializeField] private RobotHintUI robotHintUI;

    private IReadOnlyList<Transform> _waypoints;
    private int _currentWaypointIndex;
    private RobotMovementState _currentMovementState;

    private Vector3 _currentVelocity;
    private Coroutine _transitionCoroutine;

    private GameObject _player;

    public void SetWaypoints(IReadOnlyList<Transform> waypoints)
    {
        StopTransitionCoroutine();

        _waypoints = waypoints;
        _currentWaypointIndex = 0;

        _currentVelocity = Vector3.zero;

        _currentBehaviourState = RobotBehaviourState.Recycling;
        _currentMovementState = RobotMovementState.Rotating;
    }

    public void OnPlayerEnter(GameObject player)
    {
        if(_player == null)
        {
            _player = player;
        }

        _currentBehaviourState = RobotBehaviourState.Staring;
        robotHintUI.ShowUI();
    }

    public void OnPlayerExit()
    {
        _currentBehaviourState = RobotBehaviourState.Recycling;
        _currentMovementState = RobotMovementState.Rotating;
        robotHintUI.HideUI();
    }

    public void Deactivate()
    {
        robotHintUI.HideUI();
        Destroy(gameObject);
    }

    private void Start()
    {
        robotHintUI.HideUI();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        switch (_currentBehaviourState)
        {
            case RobotBehaviourState.Recycling:
                UpdateMovementState(deltaTime);
                break;
            case RobotBehaviourState.Staring:
                StareAtPlayer(deltaTime);
                break;
        }
    }

    private void UpdateMovementState(float deltaTime)
    {
        if (_waypoints == null || _currentWaypointIndex >= _waypoints.Count)
        {
            return;
        }

        switch (_currentMovementState)
        {
            case RobotMovementState.Rotating:
                RotateToCurrentWaypoint(deltaTime);
                break;
            case RobotMovementState.Moving:
                MoveToCurrentWaypoint(deltaTime);
                break;
            case RobotMovementState.Delay:
                break;
        }
    }

    private void RotateToCurrentWaypoint(float deltaTime)
    {
        Vector3 targetPosition = GetFlatWaypointPosition(_waypoints[_currentWaypointIndex]);
        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            BeginTransition(RobotMovementState.Moving);
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) <= 0.1f)
        {
            transform.rotation = targetRotation;
            BeginTransition(RobotMovementState.Moving);
        }
    }

    private void MoveToCurrentWaypoint(float deltaTime)
    {
        Vector3 targetPosition = GetFlatWaypointPosition(_waypoints[_currentWaypointIndex]);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, movementSmoothing, maxMovementSpeed);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            _currentWaypointIndex++;
            BeginTransition(RobotMovementState.Rotating);
        }
    }

    private void BeginTransition(RobotMovementState nextState)
    {
        _currentMovementState = RobotMovementState.Delay;
        StopTransitionCoroutine();
        _transitionCoroutine = StartCoroutine(WaitBeforeTransition(nextState));
    }

    private IEnumerator WaitBeforeTransition(RobotMovementState nextMovementState)
    {
        yield return new WaitForSeconds(stateTransitionDelay);
        _currentMovementState = nextMovementState;
    }

    private void StopTransitionCoroutine()
    {
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }
    }

    private Vector3 GetFlatWaypointPosition(Transform waypoint)
    {
        return new Vector3(waypoint.position.x, transform.position.y, waypoint.position.z);
    }

    private void StareAtPlayer(float deltaTime)
    {
        Vector3 direction = _player.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * deltaTime);
    }
}