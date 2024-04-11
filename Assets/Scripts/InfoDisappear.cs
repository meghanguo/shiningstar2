using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDisappear : MonoBehaviour
{
    public Canvas canvas;
    public int menuWandID = 1;
    public CAVE2.Button menuBackButton = CAVE2.Button.Button3;

    // Update is called once per frame
    void Update()
    {
        if (CAVE2.Input.GetButtonDown(menuWandID, menuBackButton))
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
