using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteraction playerInteraction;

    private InputActions _inputActions;
    private Vector2 _movementInputVector;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Movement.performed += HandleMovement;
        _inputActions.Player.Movement.canceled += HandleMovementCancel;

        _inputActions.Player.Hack.performed += HandleHack;

        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Movement.performed -= HandleMovement;
        _inputActions.Player.Movement.canceled -= HandleMovementCancel;

        _inputActions.Player.Hack.performed += HandleHack;

        _inputActions.Disable();
    }

    private void HandleMovement(InputAction.CallbackContext ctx)
    {
        _movementInputVector = ctx.ReadValue<Vector2>();
    }

    private void HandleMovementCancel(InputAction.CallbackContext ctx)
    {
        _movementInputVector = Vector2.zero;
    }

    private void HandleHack(InputAction.CallbackContext ctx)
    {
        playerInteraction.Hack();
    }

    private void Update()
    {
        playerController.UpdateMovementInputVector(_movementInputVector);
    }
}
