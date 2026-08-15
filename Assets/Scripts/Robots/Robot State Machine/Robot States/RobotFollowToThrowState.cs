using System;

public class RobotFollowToThrowState : IRobotState
{
    private readonly RobotContext _context;

    private bool _completed;

    public Action OnCompleted;

    public RobotFollowToThrowState(RobotContext context)
    {
        _context = context;
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }
    public void Tick(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

}
