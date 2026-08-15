public interface IRobotState
{
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}