using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void OnEnable()
    {
        if (!GetComponent<Renderer>().isVisible)
        {
            enabled = false;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void OnBecameInvisible()
    {
        enabled = false;
    }
}
