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
    public MeshRenderer portalScreen;

    [Header("COMPONENTS")]
    public Portal other;
    
    [HideInInspector] public UniversalAdditionalCameraData camData;
    private Camera mainCam;
    private RenderTexture camRenderTexture;
    
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
        cam.targetTexture = camRenderTexture;
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
        if (!camRenderTexture || camRenderTexture.width != Screen.width || camRenderTexture.height != Screen.height)
        {
            if (camRenderTexture != null)
            {
                camRenderTexture.Release();
            }
            camRenderTexture = new RenderTexture(Screen.width, Screen.height, 32);
            cam.targetTexture = camRenderTexture;
        }
        BindCamTexture();
        
        //Vector3 offsetWorld = other.transform.position - mainCam.transform.position;
        //Vector3 offsetLocal = other.transform.InverseTransformDirection(offsetWorld);

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
        Debug.DrawLine(transform.position, transform.position + transform.forward*10f);
    }

    private void BindCamTexture()
    {
        //Rendering the view form the other portal on the mesh
        if (other)
        {
            portalScreen.material.SetTexture("CameraTexture", other.camRenderTexture);
        }
    }
}
