using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotBehaviourController : MonoBehaviour
{
    [Header("Collection")]
    [SerializeField] private float collectionDuration = 3f;

    private RobotMovementController _movementController;
    private RobotGarbageController _garbageController;
    private RobotContext _context;

    private IRobotState _currentState;

    private IReadOnlyList<Transform> _waypoints;

    private Transform _detectedPlayer;

    private RobotFollowToCollectState _followToCollectState;
    private RobotCollectionState _collectionState;
    private RobotReturnToBaseState _returnToBaseState;

    private RobotTakeGarbageState _takeGarbageState;
    private RobotFollowToThrowState _followToThrowState;
    private RobotThrowGarbageState _throwGarbageState;

    private bool _isHacked;

    private void Awake()
    {
        _movementController = GetComponent<RobotMovementController>();
        _garbageController = GetComponent<RobotGarbageController>();

        _context = new RobotContext(
            this,
            _movementController
        );

        CreateStates();
        ConnectStates();
    }

    private void Update()
    {
        _currentState?.Tick(Time.deltaTime);
    }

    private void CreateStates()
    {
        _followToCollectState = new RobotFollowToCollectState(_context);
        _collectionState = new RobotCollectionState(_context, collectionDuration);
        _returnToBaseState = new RobotReturnToBaseState(_context);

        _takeGarbageState = new RobotTakeGarbageState(_context);
        _followToThrowState = new RobotFollowToThrowState(_context);
        _throwGarbageState = new RobotThrowGarbageState(_context);
    }

    private void ConnectStates()
    {
        _followToCollectState.OnCompleted += () =>
        {
            TransitionTo(_collectionState);
        };

        _collectionState.OnCompleted += () =>
        {
            _garbageController.SpawnGarbage();

            TransitionTo(_returnToBaseState);
        };

        _returnToBaseState.OnCompleted += OnReturnToBaseCompleted;

        _takeGarbageState.OnCompleted += () =>
        {
            SetupThrowPath();

            TransitionTo(_followToThrowState);
        };

        _followToThrowState.OnCompleted += () =>
        {
            TransitionTo(_throwGarbageState);
        };

        _throwGarbageState.OnCompleted += () =>
        {
            SetupReturnPath();

            TransitionTo(_returnToBaseState);
        };
    }

    public void SetWaypoints(IReadOnlyList<Transform> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        _waypoints = waypoints;

        _movementController.SetWaypoints(waypoints);

        _isHacked = false;

        TransitionTo(_followToCollectState);
    }

    private void OnReturnToBaseCompleted()
    {
        if (!_isHacked)
        {
            return;
        }

        TransitionTo(_takeGarbageState);
    }

    public void Hack()
    {
        if (_isHacked) return;

        _isHacked = true;
        _detectedPlayer = null;

        _garbageController.DropGarbage();

        if (_currentState == _returnToBaseState) return;

        _currentState?.Exit();

        SetupReturnPathFromCurrentPosition();

        TransitionTo(_returnToBaseState);
    }

    private void SetupReturnPath()
    {
        IReadOnlyList<Transform> returnPath = CreateReturnPath();

        _movementController.SetWaypoints(returnPath);

        _movementController.StartFollowing();
    }

    private void SetupThrowPath()
    {
        if (_waypoints == null || _waypoints.Count < 3) return;

        int throwWaypointIndex = Random.Range(1, _waypoints.Count - 1);

        IReadOnlyList<Transform> throwPath = CreatePathToWaypoint(throwWaypointIndex);

        _movementController.SetWaypoints(throwPath);

        _movementController.StartFollowing();
    }

    private void SetupReturnPathFromCurrentPosition()
    {
        if (_waypoints == null || _waypoints.Count == 0) return;

        int currentIndex = _movementController.CurrentWaypointIndex;

        currentIndex = Mathf.Clamp(currentIndex, 0, _waypoints.Count - 1);

        List<Transform> returnPath = new List<Transform>();

        for (int i = currentIndex; i >= 0; i--)
        {
            returnPath.Add(_waypoints[i]);
        }

        _movementController.SetWaypoints(returnPath);
    }

    private IReadOnlyList<Transform> CreateReturnPath()
    {
        if (_waypoints == null || _waypoints.Count <= 1) return new List<Transform>();

        return _waypoints.Take(_waypoints.Count - 1).Reverse().ToList();
    }

    private IReadOnlyList<Transform> CreatePathToWaypoint(int waypointIndex)
    {
        if (_waypoints == null || _waypoints.Count == 0)
        {
            return new List<Transform>();
        }

        waypointIndex = Mathf.Clamp(waypointIndex, 0, _waypoints.Count - 1);

        return _waypoints.Take(waypointIndex + 1).ToList();
    }

    public void OnPlayerDetected(GameObject player)
    {
        if (player == null || _isHacked) return;

        _detectedPlayer = player.transform;

        if (_currentState == _followToCollectState || _currentState == _returnToBaseState)
        {
            _movementController.FaceTarget(_detectedPlayer);
        }
    }

    public void OnPlayerLost()
    {
        _detectedPlayer = null;

        if (_isHacked) return;

        if (_currentState == _followToCollectState || _currentState == _returnToBaseState)
        {
            _movementController.ResumeFollowing();
        }
    }

    private void UpdatePlayerInteraction()
    {
        if (_detectedPlayer == null || _isHacked) return;

        if (_currentState == _followToCollectState || _currentState == _returnToBaseState)
        {
            _movementController.FaceTarget(_detectedPlayer);
        }
    }

    public void TransitionTo(IRobotState newState)
    {
        if (_currentState == newState) return;

        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();

        UpdatePlayerInteraction();
    }
}