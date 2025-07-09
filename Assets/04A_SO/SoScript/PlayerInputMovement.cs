using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "PlayerInputMovement", menuName = "Scriptable Objects/PlayerInputMovement")]
public class PlayerInputMovement : ScriptableObject, InputSystemCore.IPlayerActions
{
    private InputSystemCore _controls;
    public Vector2 Movecontext { get; private set; }
    public Vector2 Aimcontext { get; private set; }

    public Action _onJump;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new InputSystemCore();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }
    public void OnMousePos(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = context.ReadValue<Vector2>();
        Aimcontext = Camera.main.ScreenToWorldPoint(screenPosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        Movecontext = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            _onJump?.Invoke();

    }
}
