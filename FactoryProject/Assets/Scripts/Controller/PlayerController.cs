using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public Vector2 lookInput;

    [HideInInspector] public bool jumpInput;

    public static PlayerController instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        //Movement input listener
        playerInput.currentActionMap["Movement"].performed += OnMovement;
        playerInput.currentActionMap["Movement"].canceled += OnMovement;
        //Look input listener
        playerInput.currentActionMap["Look"].performed += OnLook;
        playerInput.currentActionMap["Look"].canceled += OnLook;
        
        //playerInput.currentActionMap["OnLook"].performed += OnJump;
    }

    private void OnMovement(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext value)
    {
        lookInput = value.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext value)
    {
        jumpInput = value.started;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
