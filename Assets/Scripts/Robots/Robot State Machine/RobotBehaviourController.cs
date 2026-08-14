using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotBehaviourController : MonoBehaviour
{
    [SerializeField] private float collectionDuration = 3f;

    private RobotMovementController _movementController;
    private RobotContext _context;

    private IRobotState _currentState;

    private IReadOnlyList<Transform> _waypoints;

    private Transform _detectedPlayer;

    public RobotFollowToCollectState FollowToCollectState { get; private set; }
    public RobotCollectionState CollectionState { get; private set; }
    public RobotReturnToBaseState ReturnToBaseState { get; private set; }

    private void Awake()
    {
        _movementController = GetComponent<RobotMovementController>();

        _context = new RobotContext(this, _movementController);

        FollowToCollectState = new RobotFollowToCollectState(_context);
        CollectionState = new RobotCollectionState(_context, collectionDuration);
        ReturnToBaseState = new RobotReturnToBaseState(_context);

        FollowToCollectState.OnCompleted += () => TransitionTo(CollectionState);
        CollectionState.OnCompleted += () => TransitionTo(ReturnToBaseState);
    }

    private void FixedUpdate()
    {
        _currentState?.Tick(Time.fixedDeltaTime);
    }

    public void SetWaypoints(IReadOnlyList<Transform> waypoints)
    {
        _waypoints = waypoints;

        _movementController.SetWaypoints(waypoints);

        TransitionTo(FollowToCollectState);
    }

    public void SetReturnWaypoints()
    {
        IReadOnlyList<Transform> returnPath = CreateReturnPath();

        _movementController.SetWaypoints(returnPath);
    }

    private IReadOnlyList<Transform> CreateReturnPath()
    {
        if (_waypoints == null || _waypoints.Count <= 1) return new List<Transform>();

        return _waypoints.Take(_waypoints.Count - 1).Reverse().ToList();
    }

    public void TransitionTo(IRobotState newState)
    {
        if (_currentState == newState) return;

        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();

        UpdatePlayerInteraction();
    }

    public void OnPlayerDetected(GameObject player)
    {
        if (player == null) return;

        _detectedPlayer = player.transform;

        if (_currentState == FollowToCollectState || _currentState == ReturnToBaseState)
        {
            _movementController.FaceTarget(_detectedPlayer);
        }
    }

    public void OnPlayerLost()
    {
        _detectedPlayer = null;

        if (_currentState == FollowToCollectState || _currentState == ReturnToBaseState)
        {
            _movementController.ResumeFollowing();
        }
    }

    private void UpdatePlayerInteraction()
    {
        if (_detectedPlayer == null)
            return;

        if (_currentState == FollowToCollectState ||
            _currentState == ReturnToBaseState)
        {
            _movementController.FaceTarget(_detectedPlayer);
        }
    }

    public void Hack()
    {
        _currentState?.Exit();
        Destroy(gameObject);
    }
}