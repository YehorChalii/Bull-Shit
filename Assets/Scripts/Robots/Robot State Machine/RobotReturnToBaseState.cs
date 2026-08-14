public class RobotReturnToBaseState : IRobotState
{
    private readonly RobotContext _context;

    public RobotReturnToBaseState(RobotContext context)
    {
        _context = context;
    }

    public void Enter()
    {
        _context.BehaviourController.SetReturnWaypoints();
        _context.MovementController.StartFollowing();
    }

    public void Tick(float deltaTime)
    {
        _context.MovementController.FollowPath(deltaTime);

        if (_context.MovementController.FinishedPath)
        {
            _context.BehaviourController.Hack();
        }
    }

    public void Exit()
    {
        _context.MovementController.Stop();
    }
}