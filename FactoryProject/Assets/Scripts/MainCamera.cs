using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnPreCull()
    {
        Debug.Log("cull");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
