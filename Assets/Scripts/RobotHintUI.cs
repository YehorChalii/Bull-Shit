using UnityEngine;

public class RobotHintUI : MonoBehaviour
{
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
