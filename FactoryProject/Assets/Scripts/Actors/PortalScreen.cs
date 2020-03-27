using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScreen : MonoBehaviour
{
    public MeshRenderer renderer;
    private void OnTriggerEnter(Collider other)
    {
        renderer.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        renderer.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
