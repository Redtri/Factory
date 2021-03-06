﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using System.Linq;

public class Pawn : MonoBehaviour
{
    [Header("PARAMETERS")]
    public float lookSensitivity;
    public Vector2 clampLook;

    public float moveSpeed;
    public float normalInterp;
    
    [Header("COMPONENTS")]
    public PlayerController controller;
    public Rigidbody rigidBody;
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

    private float interpTime;
    private Vector3 lastNormal;
    private Quaternion baseRotation;
    private void Move()
    {
        //TODO : Check slope limit on CharacterController
        float xMove = controller.movementInput.x;
        float yMove = controller.movementInput.y;
        
        Vector3 movement = transform.right * xMove + transform.forward * yMove;
        movement *= moveSpeed * Time.fixedDeltaTime;

        RaycastHit hit;

        
        Debug.DrawLine(transform.position, transform.position +(-transform.up + transform.forward*2f) * 4f, Color.red);
        if (Physics.Raycast(transform.position, -transform.up + transform.forward*2f, out hit,4f))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (!meshCollider || !meshCollider.sharedMesh)
                return;

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
            Transform hitTransform = hit.collider.transform;
            p0 = hitTransform.TransformPoint(p0);
            p1 = hitTransform.TransformPoint(p1);
            p2 = hitTransform.TransformPoint(p2);
            Debug.DrawLine(p0, p1, Color.red);
            Debug.DrawLine(p1, p2, Color.red);
            Debug.DrawLine(p2, p0, Color.red);

            Vector3 forward = hit.normal;
            if (forward != lastNormal)
            {
                interpTime = Time.time;
                baseRotation = transform.rotation;
                lastNormal = forward;
            }
            Vector3 center = new Vector3((p0.x + p1.x + p2.x)/3, (p0.y + p1.y + p2.y)/3, (p0.z + p1.z + p2.z)/3);
            
            Debug.DrawLine(center, center + forward * 10f, Color.cyan);
            Debug.DrawLine(center, center + Vector3.up * 10f, Color.cyan);
            //transform.rotation = Quaternion.FromToRotation(Vector3.up, -forward);
            //transform.Rotate(forward);
            //Debug.DrawLine(hit.collider.transform.position, hit.collider.transform.position + hit.collider.transform.forward * 10f);
            //rigidBody.AddForce(Vector3.Normalize(hit.normal - transform.position) * rigidBody.mass * Physics.gravity.magnitude * Time.fixedDeltaTime);
            Physics.gravity = - hit.normal * 10f;
            Quaternion rotation;
            if (Time.time - interpTime > normalInterp)
            {
                rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }
            else
            {
                rotation = Quaternion.Lerp(baseRotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, (Time.time-interpTime)/normalInterp);
            }
            
            transform.rotation = rotation;
            Debug.DrawLine(center, center - hit.normal * 10f, Color.green);
        }
        Vector3 newVelocity = transform.TransformDirection(movement);
        newVelocity = new Vector3(Mathf.Clamp(movement.x, -moveSpeed, moveSpeed), Mathf.Clamp(movement.y, -moveSpeed, moveSpeed), Mathf.Clamp(movement.z, -moveSpeed, moveSpeed));
        Debug.DrawLine(transform.position, transform.position + newVelocity, Color.black);
        rigidBody.MovePosition(transform.position + newVelocity);
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
