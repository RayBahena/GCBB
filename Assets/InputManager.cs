using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    [System.NonSerialized] public static PlayerInput PlayerInput;

    [System.NonSerialized] public static Vector2 Movement;
    [System.NonSerialized] public static bool JumpWasPressed;
    [System.NonSerialized] public static bool JumpIsHeld;
   [System.NonSerialized] public static bool JumpWasReleased;
    [System.NonSerialized] public static bool RunIsHeld;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;

    public void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();
        RunIsHeld = _runAction.IsPressed();
    }
}
