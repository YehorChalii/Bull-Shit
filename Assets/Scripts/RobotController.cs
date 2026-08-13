using System;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private RobotHintUI robotHintUI;

    private void Start()
    {
        robotHintUI.HideUI();
    }

    public void OnPlayerEnter()
    {
        robotHintUI.ShowUI();
    }

    public void OnPlayerExit()
    {
        robotHintUI.HideUI();
    }

    public void Deactivate()
    {
        robotHintUI.HideUI();
        Destroy(gameObject);
    }

}
