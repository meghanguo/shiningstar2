using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationInfo : MonoBehaviour
{
    public Canvas constellationCanvas;

    // Start is called before the first frame update
    void Start()
    {
        constellationCanvas.gameObject.SetActive(false);
    }

    public void ShowExoplanetInfo()
    {
        constellationCanvas.gameObject.SetActive(true);
    }

}
