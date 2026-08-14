using System;

public class RobotFollowToCollectState : IRobotState
{
    private readonly RobotContext _context;

    private bool _completed;

    public Action OnCompleted;

    public RobotFollowToCollectState(RobotContext context)
    {
        _context = context;
    }

    public void Enter()
    {
        _completed = false;

        _context.MovementController.StartFollowing();
    }

    public void Tick(float deltaTime)
    {
        _context.MovementController.FollowPath(deltaTime);

        if (_completed) return;

        if (_context.MovementController.FinishedPath)
        {
            _completed = true;
            OnCompleted?.Invoke();
        }
    }

    public void Exit()
    {
        _context.MovementController.Stop();
    }
}