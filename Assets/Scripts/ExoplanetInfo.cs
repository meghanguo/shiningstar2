using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExoplanetInfo : MonoBehaviour
{
    public Canvas exoplanetCanvas;

    // Start is called before the first frame update
    void Start()
    {
        exoplanetCanvas.gameObject.SetActive(false);
    }

    public void ShowExoplanetInfo()
    {
        exoplanetCanvas.gameObject.SetActive(true);
    }

    
}
