using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExoplanetDataLoader
{

    public Dictionary<string, int> LoadExoplanets()
    {
        Dictionary<string, int> exoplanets = new Dictionary<string, int>();
        var data = Resources.Load<TextAsset>("exoplanet_data");
        if (data != null)
        {
            Debug.Log("exoplanet CSV file loaded successfully!");
        }
        else
        {
            Debug.LogError("exoplanet failed to load CSV file!");
        }

        var lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            string name = values[1];
            int num_planets = int.Parse(values[2]);
            exoplanets.Add(name, num_planets);
        }
        return exoplanets;
    }

}
