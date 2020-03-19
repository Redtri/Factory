using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header("PARAMETERS")]
    public float lookSensitivity;
    public Vector2 clampLook;

    public float moveSpeed;
    
    [Header("COMPONENTS")]
    public PlayerController controller;
    public CharacterController characterController;
    public Camera cam;

    private float xRotation = 0f;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
    }

    private void Move()
    {
        float xMove = controller.movementInput.x;
        float yMove = controller.movementInput.y;
        
        Vector3 movement = transform.right * xMove + transform.forward * yMove;
        movement *= moveSpeed * Time.deltaTime;
        
        characterController.Move(movement);
    }

    private void Look()
    {
        float xLook = controller.lookInput.x * lookSensitivity * Time.deltaTime;
        float yLook = controller.lookInput.y * lookSensitivity * Time.deltaTime;

        xRotation -= yLook;
        xRotation = Mathf.Clamp(xRotation, clampLook.x, clampLook.y);
        
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * xLook);
    }
}
