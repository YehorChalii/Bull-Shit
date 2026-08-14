using System;
using UnityEngine;

public class RobotCollectionState : IRobotState
{
    private readonly RobotContext _context;
    private readonly float _collectionDuration;

    private float _timer;
    private bool _completed;
    public Action OnCompleted;

    public RobotCollectionState(RobotContext context, float collectionDuration)
    {
        _context = context;
        _collectionDuration = collectionDuration;
    }

    public void Enter()
    {
        _completed = false;
        _timer = 0f;
    }

    public void Tick(float deltaTime)
    {
        if (_completed) return;

        _timer += deltaTime;

        if (_timer >= _collectionDuration)
        {
            _completed = true;
            OnCompleted?.Invoke();
        }
    }

    public void Exit()
    {
        _timer = 0f;
    }
}