using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StarDataLoader
{
    public class Star
    {
        public string hip;
        public float dist;
        public float x0;
        public float y0;
        public float z0;
        public float mag;
        public float absmag;
        public float vx;
        public float vy;
        public float vz;
        public char spect;
        public Color color;
        public float size;

        public Star(string hip, float dist, float x0, float y0, float z0, float mag, float absmag, float vx, float vy, float vz, char spect)
        {
            this.hip = hip;
            this.dist = dist;
            this.x0 = x0;
            this.y0 = y0; 
            this.z0 = z0;
            this.mag = mag;
            this.absmag = absmag;
            this.vx = vx;
            this.vy = vy;
            this.vz = vz;
            this.spect = spect;
            color = SetColor();
            size = SetSize();
        }

        public Color SetColor()
        {
            Color IntColor(int r, int g, int b)
            {
                return new Color(r / 255f, g / 255f, b / 255f);
            }

            Color[] colors = new Color[8];
            colors[0] = IntColor(0x5c, 0x7c, 0xff);
            colors[1] = IntColor(0x5d, 0x7e, 0xff);
            colors[2] = IntColor(0x79, 0x96, 0xff);
            colors[3] = IntColor(0xb8, 0xc5, 0xff);
            colors[4] = IntColor(0xff, 0xef, 0xed);
            colors[5] = IntColor(0xff, 0xde, 0xc0);
            colors[6] = IntColor(0xff, 0xa2, 0x5a);
            colors[7] = IntColor(0xff, 0x7d, 0x24);

            int color_index = -1;
            if (spect == 'O')
            {
                color_index = 0;
            }
            else if (spect == 'B')
            {
                color_index = 1;
            }
            else if (spect == 'A')
            {
                color_index = 2;
            }
            else if (spect == 'F')
            {
                color_index = 3;
            }
            else if (spect == 'G')
            {
                color_index = 4;
            }
            else if (spect == 'K')
            {
                color_index = 5;
            }
            else if (spect == 'M')
            {
                color_index = 6;
            }

            if (color_index == -1)
            {
                return Color.white;
            }

            return Color.Lerp(colors[color_index], colors[color_index + 1], 50);
        }

        public float SetSize()
        {
            if (spect == 'O')
            {
                return 6.28f;
            }
            else if (spect == 'B')
            {
                return 4f;
            }
            else if (spect == 'A')
            {
                return 1.72f;
            }
            else if (spect == 'F')
            {
                return 1.16f;
            }
            else if (spect == 'G')
            {
                return 0.96f;
            }
            else if (spect == 'K')
            {
                return 0.76f;
            }
            else if (spect == 'M')
            {
                return 0.68f;
            }

            return 1f;
        }
    }


    public List<Star> LoadData()
    {
        List<Star> stars = new List<Star>();
        var data = Resources.Load<TextAsset>("athyg_31_reduced_m10");
        if (data != null)
        {
            Debug.Log("CSV file loaded successfully!");
        }
        else
        {
            Debug.LogError("Failed to load CSV file!");
        }

        var lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            string hip = values[0];
            float dist = float.Parse(values[1]);
            float x0 = float.Parse(values[2]);
            float y0 = float.Parse(values[3]);
            float z0 = float.Parse(values[4]);
            float mag = float.Parse(values[5]);
            float absmag = float.Parse(values[6]);
            float vx = float.Parse(values[7]);
            float vy = float.Parse(values[8]);
            float vz = float.Parse(values[9]);
            char spect = char.Parse(values[10].Trim());

            Star star = new Star(hip, dist, x0, y0, z0, mag, absmag, vx, vy, vz, spect);
            stars.Add(star);
        }
        Debug.Log("number of stars: ");
        Debug.Log(stars.Count);
        return stars; 
    }
}