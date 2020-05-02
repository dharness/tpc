using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CustomCharacterController characterController;
    CharacterAnimator _characterAnimator;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _characterAnimator = GetComponent<CharacterAnimator>();
    }

    void Update()
    {
        characterController.OnCharacterUpdate();
    }

    void FixedUpdate()
    {
        _characterAnimator.UpdateState();
        characterController.OnCharacterFixedUpdate();
    }
}
