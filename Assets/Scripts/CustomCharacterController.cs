using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MovementSettings
{
    public float Acceleration = 25.0f; // In meters/second
    public float Decceleration = 25.0f; // In meters/second
    public float MaxHorizontalSpeed = 8.0f; // In meters/second
    public float JumpSpeed = 10.0f; // In meters/second
    public float JumpAbortSpeed = 10.0f; // In meters/second
}

[System.Serializable]
public class GravitySettings
{
    public float Gravity = 20.0f; // Gravity applied when the player is airborne
    public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
    public float MaxFallSpeed = 40.0f; // The max speed at which the player can fall
}
public class CustomCharacterController : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerCamera playerCamera;
    public MovementSettings MovementSettings;
    public GravitySettings GravitySettings;
    float CameraRotationSensitivity = 3.0f;
    public float minPitchAngle = -45.0f;
    public float maxPitchAngle = 75.0f;
    public float minRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
    public float maxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)
    public bool IsGrounded { get; private set; }
    public CharacterController unityCharacterController; // The Unity's CharacterController

    private float _targetHorizontalSpeed; // In meters/second
    private float _horizontalSpeed; // In meters/second
    private float _verticalSpeed; // In meters/second
    private Vector2 _nextRotation;
    private Vector3 _lastMovement;
    private Vector3 _nextMovement;
    private bool _hasNextMovement;
    private bool _jumpInput;

    public void OnCharacterUpdate()
    {
        playerInput.UpdateInput();
        UpdateNextRotation();
        UpdateNextMovement();
        UpdateNextJump();
    }

    public void OnCharacterFixedUpdate()
    {
        UpdateState();
        playerCamera.SetPosition(this.transform.position);
        playerCamera.SetControlRotation(GetNextRotation());
    }


    private void UpdateState()
    {
        UpdateHorizontalSpeed();
        UpdateVerticalSpeed();
        var movement = GetMovementWithSpeed();

        unityCharacterController.Move(movement * Time.deltaTime);
        OrientToTargetRotation(movement.WithY(0.0f));

        IsGrounded = unityCharacterController.isGrounded;
    }

    private void UpdateHorizontalSpeed()
    {
        _targetHorizontalSpeed = _nextMovement.magnitude * MovementSettings.MaxHorizontalSpeed;
        float acceleration = _hasNextMovement ? MovementSettings.Acceleration : MovementSettings.Decceleration;

        _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * Time.deltaTime);
    }

    private void UpdateVerticalSpeed()
    {
        if (IsGrounded)
        {
            _verticalSpeed = -GravitySettings.GroundedGravity;

            if (_jumpInput)
            {
                _verticalSpeed = MovementSettings.JumpSpeed;
                IsGrounded = false;
            }
        }
        else
        {
            if (!_jumpInput && _verticalSpeed > 0.0f)
            {
                // This is what causes holding jump to jump higher than tapping jump.
                _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, MovementSettings.JumpAbortSpeed * Time.deltaTime);
            }

            _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, GravitySettings.Gravity * Time.deltaTime);
        }
    }

    private Vector3 GetMovementWithSpeed()
    {
        Vector3 moveDir = _hasNextMovement ? _nextMovement : _lastMovement;
        Vector3 movement = _horizontalSpeed * moveDir + _verticalSpeed * Vector3.up;
        return movement;
    }

    private void OrientToTargetRotation(Vector3 horizontalMovement)
    {
        if (horizontalMovement.sqrMagnitude > 0.0f)
		{
            float rotationSpeed = Mathf.Lerp(maxRotationSpeed, minRotationSpeed, _horizontalSpeed / _targetHorizontalSpeed);
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ------------ Jump ----------------------

    private void UpdateNextJump()
    {
        _jumpInput = playerInput.JumpInput;
    }


    // --------------------- ROTATION  ---------------------
    private void UpdateNextRotation()
    {
        Vector2 camInput = playerInput.CameraInput;
        Vector2 nextRotation = GetNextRotation();

        // Adjust the pitch angle (X Rotation)
        float pitchAngle = nextRotation.x; // rotation around the x axis
        pitchAngle -= camInput.y * CameraRotationSensitivity;

        // Adjust the yaw angle (Y Rotation)
        float yawAngle = nextRotation.y;
        yawAngle += camInput.x * CameraRotationSensitivity;

        nextRotation = new Vector2(pitchAngle, yawAngle);
        SetNextRotation(nextRotation);
    }

    public Vector2 GetNextRotation()
    {
        return _nextRotation;
    }

    public void SetNextRotation(Vector2 nextRotation)
    {
        // Adjust the pitch angle (X Rotation)
        float pitchAngle = nextRotation.x;
        pitchAngle %= 360.0f;
        pitchAngle = Mathf.Clamp(pitchAngle, minPitchAngle, maxPitchAngle);

        // Adjust the yaw angle (Y Rotation)
        float yawAngle = nextRotation.y;
        yawAngle %= 360.0f;

        _nextRotation = new Vector2(pitchAngle, yawAngle);
    }


    // --------------------- MOVEMENT --------------------- 
    private void UpdateNextMovement()
    {
        SetNextMovement(GetNextMovement());
    }

    private Vector3 GetNextMovement()
	{
        // Calculate the move direction relative to the character's yaw rotation
        Quaternion yawRotation = Quaternion.Euler(0.0f, GetNextRotation().y, 0.0f);
        Vector3 forward = yawRotation * Vector3.forward;
        Vector3 right = yawRotation * Vector3.right;
        Vector3 nextMovement = (forward * playerInput.MoveInput.y + right * playerInput.MoveInput.x);

        if (nextMovement.sqrMagnitude > 1f)
        {
            nextMovement.Normalize();
        }

        return nextMovement;
    }

    public void SetNextMovement(Vector3 nextMovement)
    {
        bool hasNextMovement = nextMovement.sqrMagnitude > 0.0f;

        if (_hasNextMovement && !hasNextMovement)
        {
            _lastMovement = _nextMovement;
        }

        _nextMovement = nextMovement;
        _hasNextMovement = hasNextMovement;
    }
}

