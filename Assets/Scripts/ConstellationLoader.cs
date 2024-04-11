using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationLoader
{
    public class Constellation
    {
        public string name;
        public List<string> hips;

        public Constellation(string name, List<string> hips)
        {
            this.name = name;
            this.hips = hips;
        }
    }

    public List<Constellation> LoadConstellations(string fileName)
    {
        List<Constellation> constellations = new List<Constellation>();
        var data = Resources.Load<TextAsset>(fileName);

        var lines = data.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(' ');
            string name = values[0];
            List<string> hips = new List<string>();

            for (int index = 1; index < values.Length; index++)
            {
                hips.Add(values[index]);
            }

            Constellation constellation = new Constellation(name, hips);
            constellations.Add(constellation);
        }

        return constellations;
    }
}
