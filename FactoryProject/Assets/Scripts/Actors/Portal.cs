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
    public Portal otherPortal;
    
    
    [HideInInspector] public UniversalAdditionalCameraData camData;
    private Camera mainCam;
    private RenderTexture camRenderTexture;

    private List<TravelEntity> travellers;
    
    
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
        
        travellers = new List<TravelEntity>();
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

    private void FixedUpdate()
    {
        if(travellers.Count > 0)
            Debug.Log(name + " containing travellers");
        for (int i = 0; i < travellers.Count; i++)
        {
            TravelEntity entity = travellers[i];
            Transform entityT = entity.transform;

            Vector3 newOffset = entityT.position - transform.position;

            int oldSign = Math.Sign(Vector3.Dot(entity.prevOffsetToPortal, transform.forward));
            int newSign = Math.Sign(Vector3.Dot(newOffset, transform.forward));

            if (oldSign != newSign) {
                //Debug.Log("Old was : " + oldSign + ". New is " + newSign);
                    Matrix4x4 mat = otherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * entityT.localToWorldMatrix;
                    entity.Teleport(transform, otherPortal.transform, mat.GetColumn(3), mat.rotation);
                
                    otherPortal.OnTravellerEnter(entity);
                    travellers.RemoveAt(i);
                    i--;
            }else
                entity.prevOffsetToPortal = newOffset;
        }
        ProtectScreenFromClipping(mainCam.transform.position);
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

        if (otherPortal != null)
        {
            Matrix4x4 mat = transform.localToWorldMatrix * otherPortal.transform.worldToLocalMatrix * mainCam.transform.localToWorldMatrix;
            cam.transform.SetPositionAndRotation(mat.GetColumn(3), mat.rotation);
        }
        /*
        cam.transform.localPosition = offsetLocal;
        cam.transform.LookAt(transform.position);
        */
    }
    
    
    // Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
    float ProtectScreenFromClipping (Vector3 viewPoint) {
        float halfHeight = mainCam.nearClipPlane * Mathf.Tan (mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * mainCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, mainCam.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = portalScreen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot (transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3 (screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    public void OnTravellerEnter(TravelEntity entity)
    {
        Debug.Log("Traveller enters " + name);
        if (!travellers.Contains(entity)) {
            travellers.Add(entity);
            entity.prevOffsetToPortal = entity.transform.position - transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TravelEntity entity = other.GetComponent<TravelEntity>();

        if (entity != null) {
            OnTravellerEnter(entity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TravelEntity entity = other.GetComponent<TravelEntity>();

        if (entity && travellers.Contains(entity))  {
            travellers.Remove(entity);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward*10f);
    }

    private void BindCamTexture()
    {
        //Rendering the view form the other portal on the mesh
        if (otherPortal)
        {
            portalScreen.material.SetTexture("CameraTexture", otherPortal.camRenderTexture);
        }
    }
}
