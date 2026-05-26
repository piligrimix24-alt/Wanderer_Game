using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions _playerInputActions;
    private bool _isDisposed = false;

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerShift;
    public event EventHandler OnPlayerShiftReleased;
    public event EventHandler OnPlayerInteractionE;
    public event EventHandler OnPlayerInteractionEStopped;
    //public event EventHandler OnPlayerJump;
    //=================================================================================================================
    private void Awake()
    {
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _playerInputActions.Combat.Attack.started += PlayerAttack_started;
        _playerInputActions.Player.Shift.started += Shift_started;
        _playerInputActions.Player.Shift.canceled += Shift_canceled;
        _playerInputActions.Player.InteractionE.started += InteractionE_started;
        _playerInputActions.Player.InteractionE.canceled += InteractionE_canceled;
    }
    private void OnDestroy()
    {
        Dispose();
    }

    private void OnApplicationQuit()
    {
        Dispose();
    }

    private void Dispose()
    {
        if (_isDisposed) return;

        if (_playerInputActions != null)
        {
            _playerInputActions.Combat.Attack.started -= PlayerAttack_started;
            _playerInputActions.Player.Shift.started -= Shift_started;
            _playerInputActions.Player.Shift.canceled -= Shift_canceled;
            _playerInputActions.Player.InteractionE.started -= InteractionE_started;
            _playerInputActions.Player.InteractionE.canceled -= InteractionE_canceled;

            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }

        _isDisposed = true;
    }
    //=================================================================================================================
    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }
    public Vector3 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }
    public void DisableMovement()
    {
        _playerInputActions.Disable();
    }
    public void EnableMovement()
    {
        _playerInputActions.Enable();
    }
    //=================================================================================================================
    private void Shift_canceled(InputAction.CallbackContext obj)
    {
        OnPlayerShiftReleased?.Invoke(this, EventArgs.Empty);
    }
    private void Shift_started(InputAction.CallbackContext obj)
    {
        OnPlayerShift?.Invoke(this, EventArgs.Empty);
    }
    private void PlayerAttack_started(InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }
    private void InteractionE_started(InputAction.CallbackContext context)
    {
        OnPlayerInteractionE?.Invoke(this, EventArgs.Empty);
    }
    private void InteractionE_canceled(InputAction.CallbackContext context)
    {
        OnPlayerInteractionEStopped?.Invoke(this, EventArgs.Empty);
    }
    //private void Jump_performed(InputAction.CallbackContext context)
    //{
    //    OnPlayerJump?.Invoke(this, EventArgs.Empty);
    //}
}

