using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CustomCharacterController characterController;

    void Start()
    {
        
    }

    void Update()
    {
        characterController.OnCharacterUpdate();
    }

    void FixedUpdate()
    {
        characterController.OnCharacterFixedUpdate();
    }

    
}
