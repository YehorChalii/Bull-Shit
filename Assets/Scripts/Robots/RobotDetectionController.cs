using UnityEngine;

public class RobotDetectionController : MonoBehaviour
{
    [SerializeField] private RobotHintUI hintUI;
    private RobotBehaviourController _behaviourController;

    private void Awake()
    {
        hintUI.enabled = false;
        _behaviourController = GetComponent<RobotBehaviourController>();
    }

    public void OnPlayerEnter(GameObject player)
    {
        hintUI.enabled = true;
        hintUI.SetHintUIActive(true);
        _behaviourController.OnPlayerDetected(player);
    }

    public void OnPlayerExit()
    {
        hintUI.SetHintUIActive(false);
        hintUI.enabled = false;
        _behaviourController.OnPlayerLost();
    }

    public void Hack()
    {
        hintUI.SetHintUIActive(false);
        hintUI.enabled = false;
        _behaviourController.Hack();
    }
}
