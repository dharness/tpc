using UnityEngine;

public static class CharacterAnimatorParamId
{
    public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
    public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
}

public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;
    private CustomCharacterController _customCharacterController;

    private void Awake()
    {
        _customCharacterController = GetComponent<CustomCharacterController>();
    }

    public void UpdateState()
    {
        float normHorizontalSpeed = (
            _customCharacterController.HorizontalVelocity.magnitude /
            _customCharacterController.MovementSettings.MaxHorizontalSpeed
        );
        Debug.Log(normHorizontalSpeed);
        animator.SetFloat(CharacterAnimatorParamId.HorizontalSpeed, normHorizontalSpeed);

        float jumpSpeed = _customCharacterController.MovementSettings.JumpSpeed;
        float normVerticalSpeed = _customCharacterController.VerticalVelocity.y.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);
        animator.SetFloat(CharacterAnimatorParamId.VerticalSpeed, normVerticalSpeed);

        animator.SetBool(CharacterAnimatorParamId.IsGrounded, _customCharacterController.IsGrounded);
    }
}
