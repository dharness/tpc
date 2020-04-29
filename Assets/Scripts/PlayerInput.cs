using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    public float MoveAxisDeadZone = 0.2f;
    public PlayerControls playerControls;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LastMoveInput { get; private set; }
    public Vector2 CameraInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool ChangeClothesInput { get; set; }

    public bool HasMoveInput { get; private set; }
    public Action onChangeClothesButtonPressed;

    public void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Gameplay.ChangeClothes.performed += ctx => this.HandleChangeClothesButtonPressed();
    }

    public void HandleChangeClothesButtonPressed()
    {
        //ChangeClothesInput = playerControls.Gameplay.ChangeClothes.ReadValue<float>() > 0f;
        onChangeClothesButtonPressed?.Invoke();
    }

    public void UpdateInput()
    {
        // Update MoveInput
        Vector2 moveInput = playerControls.Gameplay.Movement.ReadValue<Vector2>();
        if (Mathf.Abs(moveInput.x) < MoveAxisDeadZone)
        {
            moveInput.x = 0.0f;
        }

        if (Mathf.Abs(moveInput.y) < MoveAxisDeadZone)
        {
            moveInput.y = 0.0f;
        }

        bool hasMoveInput = moveInput.sqrMagnitude > 0.0f;

        if (HasMoveInput && !hasMoveInput)
        {
            LastMoveInput = MoveInput;
        }

        MoveInput = moveInput;
        HasMoveInput = hasMoveInput;

        // Update other inputs
        CameraInput = playerControls.Gameplay.CameraRotation.ReadValue<Vector2>();
        JumpInput = playerControls.Gameplay.Jump.ReadValue<float>() > 0f;
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
}
