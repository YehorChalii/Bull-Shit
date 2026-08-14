using UnityEngine;

public class RobotHintUI : MonoBehaviour
{
    private Quaternion _initialRotation;

    private void Awake()
    {
        SetHintUIActive(false);
        _initialRotation = Quaternion.Euler(90, 0, 0);
    }

    public void SetHintUIActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void LateUpdate()
    {
        transform.rotation = _initialRotation;
    }
}