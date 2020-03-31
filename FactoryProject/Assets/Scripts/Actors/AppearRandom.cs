using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AppearRandom : MonoBehaviour
{
    public float distance;
    
    public Vector2 durationRange;
    private Vector3 finalRotation;
    private Vector3 startRotation;
    private Vector3 finalPosition;
    private Vector3 startPosition;

    private static float duration;
    private float startTime;
    private MeshRenderer mesh;

    private bool appeared;

    private bool appearing;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        TryGetComponent(out mesh);
        appeared = false;
        appearing = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Appear();
        }
        if (appearing)
        {
            if (Time.time - startTime < duration)
            {
                float percent = (Time.time - startTime) / duration;

                if (mesh)
                {
                    foreach (Material mat in mesh.materials)
                    {
                        mat.SetFloat("Percent", 1f-(percent));
                    }
                }
                
                transform.position = Vector3.Lerp(startPosition, finalPosition, percent);
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(finalRotation), percent);
            }
            else
            {
                appeared = false;
                appearing = false;
                transform.position = finalPosition;
                transform.rotation = Quaternion.Euler(finalRotation);
            }
        }
    }

    public void Appear()
    {
        if (!appearing && !appeared)
        {
            duration = Random.Range(durationRange.x, durationRange.y);
            finalPosition = transform.position;
            finalRotation = transform.rotation.eulerAngles;
            
            transform.position += Random.insideUnitSphere * distance;
            transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 360f),Random.Range(0f, 360f),Random.Range(0f, 360f) ));

            startPosition = transform.position;
            startRotation = transform.rotation.eulerAngles;
            
            appearing = true;
            startTime = Time.time;
        }
    }
}
