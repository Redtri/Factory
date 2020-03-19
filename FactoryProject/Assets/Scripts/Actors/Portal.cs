using System.Collections;
using System.Collections.Generic;
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
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        camData = cam.GetComponent<UniversalAdditionalCameraData>();
        camData.cameraOutput = CameraOutput.Texture;
        cam.targetTexture = camRender;
        BindCamTexture();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCam();
    }

    private void UpdateCam()
    {
        Matrix4x4 mat = transform.localToWorldMatrix * other.transform.localToWorldMatrix * Pawn.instance.cam.transform.localToWorldMatrix;
        
        cam.transform.SetPositionAndRotation(mat.GetColumn(2), mat.rotation);
    }
    
    private void BindCamTexture()
    {
        if(other)
            portalMesh.material.SetTexture("CameraTexture", other.camRender);
        else
            portalMesh.material.SetTexture("CameraTexture", camRender);
    }
}
