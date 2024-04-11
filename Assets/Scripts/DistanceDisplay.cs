using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class DistanceDisplay : MonoBehaviour
{
    public GameObject text;
    public GameObject player;
    private Vector3 sol = new Vector3(0, 1f, 0);

    // Update is called once per frame
    void Update()
    {
        //Vector3 distance = gameObject.transform.position - sol;        
        text.GetComponent<TextMeshPro>().text = $"Distance from Sol: {Math.Round(Vector3.Distance(player.transform.position, sol), 2)} pc";
    }
}
