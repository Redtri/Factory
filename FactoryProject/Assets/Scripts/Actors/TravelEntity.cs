using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelEntity : MonoBehaviour
{
    public Vector3 prevOffsetToPortal { get; set; }

    public virtual void Teleport(Transform srcPortal, Transform destPortal, Vector3 pos, Quaternion rot)
    {
        Debug.Log("Teleport from " + srcPortal.name + " to " + destPortal.name);
        transform.position = pos;
        transform.rotation = rot;
    }

    public void TryEnter()
    {
        
    }

    public void TryExit()
    {
        
    }
}
