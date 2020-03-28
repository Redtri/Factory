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

    private Pickable pickable;

    private float xRotation = 0f;
    private bool teleporting;
    private Transform fromPortal;
    private Transform toPortal;

    private bool wasInteracting;

    public static Pawn instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Init()
    {
        wasInteracting = false;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Move();
        Pick();
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

    private void Pick()
    {
        if (controller.interactInput != wasInteracting)
        {
            if (wasInteracting)
            {
                if (pickable != null)
                {
                    pickable.Release();
                }
            } else
            {
                RaycastHit hit;

                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 10f))
                {
                    Pickable pick;
                    hit.collider.TryGetComponent<Pickable>(out pick);

                    if (pick)
                    {
                        pickable = pick;
                        pickable.Pick(cam.transform);
                    }
                }
            }
            wasInteracting = controller.interactInput;
        }
    }
    
    public IEnumerator Teleport(Portal from, Portal to)
    {
        if (!teleporting)
        {
            teleporting = true;
            fromPortal = from.transform;
            toPortal = to.transform;
            yield return new WaitForSeconds(.1f);
            teleporting = false;
        }
        yield return null;
    }
}
