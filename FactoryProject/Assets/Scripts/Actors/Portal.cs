using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Portal : MonoBehaviour
{
    [Header("PARAMETERS")]
    public Camera cam;
    public RenderTexture camRender;
    public MeshRenderer portalMesh;

    [Header("COMPONENTS")]
    public Portal other;
    
    [HideInInspector] public UniversalAdditionalCameraData camData;
    private Camera mainCam;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        mainCam = Camera.main;
        camData = cam.GetComponent<UniversalAdditionalCameraData>();
        camData.cameraOutput = CameraOutput.Texture;
        cam.targetTexture = camRender;
    }

    private void OnDestroy()
    {
        cam.targetTexture?.Release();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCam();
    }

    private void UpdateCam()
    {
        if (!camRender || camRender.width != Screen.width || camRender.height != Screen.height)
        {
            if (camRender != null)
            {
                camRender.Release();
            }
            camRender = new RenderTexture(Screen.width, Screen.height, 32);
            cam.targetTexture = camRender;
        }
        BindCamTexture();
        
        Vector3 offsetWorld = other.transform.position - mainCam.transform.position;
        Vector3 offsetLocal = other.transform.InverseTransformDirection(offsetWorld);

        if (other != null)
        {
            Matrix4x4 mat = transform.localToWorldMatrix * other.transform.worldToLocalMatrix * mainCam.transform.localToWorldMatrix;
            cam.transform.SetPositionAndRotation(mat.GetColumn(3), mat.rotation);
        }
        /*
        cam.transform.localPosition = offsetLocal;
        cam.transform.LookAt(transform.position);
        */
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Debug.DrawLine(transform.position, transform.position + transform.forward*10f);
    }

    private void BindCamTexture()
    {
        if(other)
            portalMesh.material.SetTexture("CameraTexture", other.camRender);
        else
            portalMesh.material.SetTexture("CameraTexture", camRender);
    }
}
