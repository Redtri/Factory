using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    private float originalFrustumHeight;
    private Vector3 originalScale;
    private float originalDist;

    private bool picked;
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        picked = false;
        originalFrustumHeight = 2.0f * Vector3.Distance(Camera.main.gameObject.transform.position, transform.position) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        originalScale = transform.localScale;
        originalDist = Vector3.Distance(Camera.main.transform.position, transform.position);
    }

    private void Update()
    {
        if (picked)
        {
            float newFrustumHeight = 2.0f * Vector3.Distance(Camera.main.gameObject.transform.position, transform.position) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float scaleFactor = newFrustumHeight / originalFrustumHeight;
            gameObject.transform.localScale = originalScale * scaleFactor;
        }
        
        
    }

    public void Pick(Transform trsf)
    {
        Vector3 pos = transform.position;
        picked = true;
        transform.parent = trsf;
        transform.position = pos;
        gameObject.layer = LayerMask.NameToLayer("Liminal");
    }

    public void Release()
    {
        //TODO : Change position of the object by raycasting the closest collision, ground, wall and so on
        float newFrustumHeight = 2.0f * Vector3.Distance(Camera.main.gameObject.transform.position, transform.position) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float scaleFactor = newFrustumHeight / originalFrustumHeight;
        gameObject.transform.localScale = originalScale * scaleFactor;
        picked = false;
        transform.parent = null;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
