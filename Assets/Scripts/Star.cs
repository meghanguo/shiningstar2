using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

// TODO how to load more stars
// TODO size

// store star info
public class StarInfo : MonoBehaviour
{
    public float x0;
    public float y0;
    public float z0;
    public float vx;
    public float vy;
    public float vz;
    public Color starColor;
    public Color exoplanetColor;
}

// component for constellation lines to follow the star movements 
public class StarFollower : MonoBehaviour
{
    public GameObject starToFollow1;
    public GameObject starToFollow2;
}

public class Star : MonoBehaviour
{
    private List<StarDataLoader.Star> stars;
    private List<GameObject> starObjects;
    private Dictionary<string, GameObject> hipStarObjects;
    private Dictionary<string, int> exoplanets;
    private List<GameObject> exoplanetObjects;
    private readonly Dictionary<string, GameObject> _cultures = new Dictionary<string, GameObject>();
    private List<GameObject> constellationLines;

    public GameObject constellationMenu;
    public GameObject exoplanetMenu;
    public GameObject starPrefab;
    public Shader starShader;
    public GameObject linePrefab;
    public Shader constellationShader;
    public GameObject yearsText;
    public Text scaleLabel;
    public Slider scaleSlider;

    // menus 
    public GameObject menu; // see different constellations
    public GameObject menu2; // switch between star and exoplanet modes
    public GameObject menu3; // evolution over time 
    public GameObject menu4; // reset location and time 
    public GameObject togglePrefab;

    private GameObject _starsRoot;
    private int currConstellation;

    // time to track movement 
    private float year;
    private int movement;

    public CAVE2WandNavigator navController;

    public GameObject evolutionMenu;

    // constellation canvases
    public Canvas noConstellationCanvas;
    public Canvas modernCanvas;
    public Canvas hawaiianCanvas;
    public Canvas navajoCanvas;
    public Canvas samoanCanvas;
    public Canvas egyptianCanvas;

    void Start()
    {
        // listener for scale slider 
        scaleSlider.onValueChanged.AddListener(delegate
        {
            changeScaling();
        });

        // constellation canvases
        noConstellationCanvas.gameObject.SetActive(false);
        modernCanvas.gameObject.SetActive(false);
        hawaiianCanvas.gameObject.SetActive(false);
        navajoCanvas.gameObject.SetActive(false);
        samoanCanvas.gameObject.SetActive(false);
        egyptianCanvas.gameObject.SetActive(false);

        // initiate values for star evolution
        year = 0;
        movement = 0;

        // set nav mode to fly
        navController.navMode = CAVE2WandNavigator.NavigationMode.Freefly;

        _starsRoot = new GameObject();
        _starsRoot.SetActive(false);
        _starsRoot.transform.position = new Vector3(0, 1f, 0);
        _starsRoot.name = "stars";

        // Sol 
        var sol = Instantiate(starPrefab, _starsRoot.transform.position, Quaternion.identity);
        sol.name = "Sol";
        sol.AddComponent<Billboard>();
        Material sun_material = sol.GetComponent<MeshRenderer>().material;
        sun_material.color = new Color(0xff / 255f, 0x57 / 255f, 0x00);
        sol.transform.localScale = new Vector3(1f, 1f, 1f);

        // load exoplanets
        ExoplanetDataLoader edl = new ExoplanetDataLoader();
        exoplanets = edl.LoadExoplanets();
        exoplanetObjects = new List<GameObject>();

        // load stars 
        StarDataLoader sdl = new StarDataLoader();
        stars = sdl.LoadData();
        hipStarObjects = new Dictionary<string, GameObject>();
        starObjects = new List<GameObject>();

        // load constellations
        constellationLines = new List<GameObject>();
        ConstellationLoader cl = new ConstellationLoader();
        Dictionary<string, List<ConstellationLoader.Constellation>> all_cultures = new Dictionary<string, List<ConstellationLoader.Constellation>>();
        all_cultures.Add("Modern_constellations", cl.LoadConstellations("modern_constellations"));
        all_cultures.Add("Hawaiian_constellations", cl.LoadConstellations("hawaiian_constellations"));
        all_cultures.Add("Navajo_constellations", cl.LoadConstellations("navajo_constellations"));
        all_cultures.Add("Samoan_constellations", cl.LoadConstellations("samoan_constellations"));
        all_cultures.Add("Egyptian_constellations", cl.LoadConstellations("egyptian_constellations"));

        // add constellation hips to a set
        HashSet<string> constellationHips = new HashSet<string>();
        foreach (KeyValuePair<string, List<ConstellationLoader.Constellation>> kvp in all_cultures)
        {
            var culture = kvp.Value;

            foreach (ConstellationLoader.Constellation constellation in culture)
            {
                List<string> all_hips = constellation.hips;
                for (int i = 0; i < all_hips.Count; i ++)
                {
                    constellationHips.Add(all_hips[i]);
                }
            }
        }
        Debug.Log($"Hashset size: {constellationHips.Count}");

        // create star objects
        int curr = 1;
        foreach (StarDataLoader.Star star in stars)
        {
            if ((star.hip != "" && star.dist < 100f) || (constellationHips.Contains(star.hip)))
            {
                var stargo = Instantiate(starPrefab, _starsRoot.transform.position, Quaternion.identity);
                stargo.transform.position = new Vector3(star.x0, star.z0 + 1, star.y0);
                StarInfo starInfo = stargo.AddComponent<StarInfo>();
                starInfo.x0 = star.x0;
                starInfo.y0 = star.y0;
                starInfo.z0 = star.z0;
                starInfo.vx = star.vx;
                starInfo.vy = star.vy;
                starInfo.vz = star.vz;
                starInfo.starColor = star.color;

                // set exoplanet color
                if (star.hip != "" && exoplanets.ContainsKey(star.hip))
                {
                    starInfo.exoplanetColor = ExoplanetColors(exoplanets[star.hip]);
                } else
                {
                    starInfo.exoplanetColor = new Color(0xff, 0xff, 0xff);
                }
                if (star.hip == "")
                {
                    stargo.name = $"Star {curr}";
                } else
                {
                    stargo.name = star.hip;
                }
                stargo.AddComponent<Billboard>();
                Material material = stargo.GetComponent<MeshRenderer>().material;
                material.color = star.color;
                stargo.transform.localScale = new Vector3(star.size, star.size, star.size);

                // all stars objects 
                starObjects.Add(stargo);

                // stars objects with hip numbers
                if (star.hip != "")
                {
                    hipStarObjects.Add(star.hip, stargo);
                }
                curr += 1;
            }
        }
        CreateExoplanetMenuItem(0);
        CreateExoplanetMenuItem(1);
        Debug.Log($"Number of star objects: {curr}");

        foreach (KeyValuePair<string, List<ConstellationLoader.Constellation>> kvp in all_cultures)
        {
            var name = kvp.Key;
            var culture = kvp.Value;

            var root = new GameObject { name = name };
            root.SetActive(false);
            root.transform.position = new Vector3(0, 0, 0);
            CreateConstellationMenuItem("No constellations", 0);

            foreach (ConstellationLoader.Constellation constellation in culture)
            {
                List<string> all_hips = constellation.hips;
                for (int i = 0; i < all_hips.Count; i += 2)
                {
                    if (hipStarObjects.ContainsKey(all_hips[i]) && hipStarObjects.ContainsKey(all_hips[i + 1]))
                    {
                        GameObject star1 = hipStarObjects[all_hips[i]];
                        GameObject star2 = hipStarObjects[all_hips[i + 1]];
                        if (star1 != null && star2 != null)
                        {
                            GameObject line = Instantiate(linePrefab);
                            line.transform.parent = root.transform;
                            line.name = $"{constellation.name} line {i}";
                            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                            lineRenderer.positionCount = 2;
                            lineRenderer.widthMultiplier = 0.2f;
                            lineRenderer.SetPosition(0, star1.transform.position);
                            lineRenderer.SetPosition(1, star2.transform.position);
                            StarFollower lineFollower = line.AddComponent<StarFollower>();
                            lineFollower.starToFollow1 = star1;
                            lineFollower.starToFollow2 = star2;
                            constellationLines.Add(line);
                        }
                    }
                }
            }
            _cultures.Add(name, root);
            CreateConstellationMenuItem(name, _cultures.Count);
        }

        // initially load modern constellations
        DrawConstellations("Modern_constellations");
        currConstellation = 1;

        // load menu for evolution over time
        CreateEvolutionItem(0);
        CreateEvolutionItem(1);
        CreateEvolutionItem(2);
    }

    // create items in evolution menu
    private GameObject CreateEvolutionItem(int index)
    {
        var item = Instantiate(togglePrefab, evolutionMenu.transform);
        var toggle = item.GetComponent<Toggle>();
        toggle.group = evolutionMenu.GetComponent<ToggleGroup>();
        toggle.onValueChanged.AddListener((value) =>
        {
            if (value) movement = index;
        });
        menu3.GetComponent<OMenu>().menuItems[index] = item.GetComponent<Selectable>();
        item.GetComponent<RectTransform>().localPosition +=
            new Vector3(0.7f * (index / 15), (-0.0804f * (index % 15)), 0.0f);
        if (index == 0)
        {
            item.GetComponentInChildren<Text>().text = "No movement";
        } else if (index == 1)
        {
            item.GetComponentInChildren<Text>().text = "Move time forward";
        } else
        {
            item.GetComponentInChildren<Text>().text = "Move time backward";
        }
        return item;
    }

    // updates every frame
    private void Update()
    {
        if (movement == 1)
        {
            year += Time.deltaTime * 1000;
            foreach (GameObject starObject in starObjects)
            {
                StarInfo starInfo = starObject.GetComponent<StarInfo>();
                Vector3 newPosition = starObject.transform.position;
                newPosition.x += starInfo.vx * Time.deltaTime;
                newPosition.y += starInfo.vz * Time.deltaTime;
                newPosition.z += starInfo.vy * Time.deltaTime;
                starObject.transform.position = newPosition;
                //starObject.transform.position = starObject.transform.position + new Vector3(Vector3.right * starInfo.vx * Time.deltaTime, star.z0 + 1, star.y0);
                //starObject.transform.Translate(Vector3.right * starInfo.vx * Time.deltaTime);
                //starObject.transform.Translate(Vector3.up * starInfo.vz * Time.deltaTime);
                //starObject.transform.Translate(Vector3.forward * starInfo.vy * Time.deltaTime);
            }
        }
        else if (movement == 2)
        {
            year -= Time.deltaTime * 1000;
            foreach (GameObject starObject in starObjects)
            {
                StarInfo starInfo = starObject.GetComponent<StarInfo>();
                Vector3 newPosition = starObject.transform.position;
                newPosition.x += starInfo.vx * Time.deltaTime * -1;
                newPosition.y += starInfo.vz * Time.deltaTime * -1;
                newPosition.z += starInfo.vy * Time.deltaTime * -1;
                starObject.transform.position = newPosition;
                //StarInfo starInfo = starObject.GetComponent<StarInfo>();
                //starObject.transform.Translate(Vector3.right * starInfo.vx * Time.deltaTime * -1);
                //starObject.transform.Translate(Vector3.up * starInfo.vz * Time.deltaTime * -1);
                //starObject.transform.Translate(Vector3.forward * starInfo.vy * Time.deltaTime * -1);
            }
        }
        if (movement == 1 || movement == 2)
        {
            foreach (GameObject line in constellationLines)
            {
                StarFollower starFollower = line.GetComponent<StarFollower>();
                LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, starFollower.starToFollow1.transform.position);
                lineRenderer.SetPosition(1, starFollower.starToFollow2.transform.position);
            }
        }
        // update years text
        yearsText.GetComponent<TextMeshPro>().text = $"Time elapsed: {Math.Round(year, 0)} years";
    }

    // assign exoplanet colors 
    public Color ExoplanetColors(int num_planets)
    {
        Color IntColor(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        Color[] colors = new Color[8];
        colors[0] = IntColor(0x00, 0x00, 0xff);
        colors[1] = IntColor(0x00, 0xff, 0x00);
        colors[2] = IntColor(0xff, 0xff, 0x00);
        colors[3] = IntColor(0xff, 0x00, 0xff);
        colors[4] = IntColor(0xff, 0x00, 0x00);
        colors[5] = IntColor(0x00, 0xff, 0xff);

        int color_index = -1;
        if (num_planets == 1)
        {
            color_index = 0;
        } else if (num_planets == 2)
        {
            color_index = 1;
        } else if (num_planets == 3)
        {
            color_index = 2;
        } else if (num_planets == 4)
        {
            color_index = 3;
        } else if (num_planets == 5)
        {
            color_index = 4;
        } else if (num_planets == 6)
        {
            color_index = 5;
        }
        return colors[color_index];
    }

    public void ChangeToExoplanet(int index)
    {
        if (index == 0)
        {
            foreach (GameObject starObject in starObjects)
            {
                Material material = starObject.GetComponent<MeshRenderer>().material;
                StarInfo starInfo = starObject.GetComponent<StarInfo>();
                material.color = starInfo.starColor;
            }
        } else
        {
            foreach (GameObject starObject in starObjects)
            {
                Material material = starObject.GetComponent<MeshRenderer>().material;
                StarInfo starInfo = starObject.GetComponent<StarInfo>();
                material.color = starInfo.exoplanetColor;
            }
        }
    }

    private GameObject CreateExoplanetMenuItem(int index)
    {
        var item = Instantiate(togglePrefab, exoplanetMenu.transform);
        var toggle = item.GetComponent<Toggle>();
        toggle.group = exoplanetMenu.GetComponent<ToggleGroup>();
        toggle.onValueChanged.AddListener((value) =>
        {
            if (value) ChangeToExoplanet(index);
        });
        menu2.GetComponent<OMenu>().menuItems[index] = item.GetComponent<Selectable>();
        item.GetComponent<RectTransform>().localPosition +=
            new Vector3(0.7f * (index / 15), (-0.0804f * (index % 15)), 0.0f);
        if (index == 0)
        {
            item.GetComponentInChildren<Text>().text = "Stars";
        } else
        {
            item.GetComponentInChildren<Text>().text = "Exoplanets";
        }
        return item;
    }

    public void DrawConstellations(string culture)
    {
        foreach (var x in _cultures)
        {
            x.Value.SetActive(x.Key == culture);
        }
    }

    private GameObject CreateConstellationMenuItem(string culture, int index)
    {
        var item = Instantiate(togglePrefab, constellationMenu.transform);
        var toggle = item.GetComponent<Toggle>();
        toggle.group = constellationMenu.GetComponent<ToggleGroup>();
        toggle.onValueChanged.AddListener((value) =>
        {
            if (value) DrawConstellations(culture);
            currConstellation = index;
        });
        menu.GetComponent<OMenu>().menuItems[index] = item.GetComponent<Selectable>();
        item.GetComponent<RectTransform>().localPosition +=
            new Vector3(0.7f * (index / 15), (-0.0804f * (index % 15)), 0.0f);
        item.GetComponentInChildren<Text>().text = culture;

        return item;
    }

    public void resetAll()
    {
        year = 0;
        foreach(GameObject starObject in starObjects)
        {
            StarInfo starInfo = starObject.GetComponent<StarInfo>();
            starObject.transform.position = new Vector3(starInfo.x0, starInfo.z0 + 1, starInfo.y0);
        }
        foreach(GameObject constellationLine in constellationLines)
        {
            StarFollower starFollower = constellationLine.GetComponent<StarFollower>();
            LineRenderer lineRenderer = constellationLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, starFollower.starToFollow1.transform.position);
            lineRenderer.SetPosition(1, starFollower.starToFollow2.transform.position);
        }
    }

    public void changeScaling()
    {
        float sliderValue = scaleSlider.value;
        //float scaleValue = Mathf.Pow(10, (sliderValue - 4));
        scaleLabel.text = $"Scale Mapping: {Math.Round(sliderValue,2)}x";

        foreach (GameObject starObject in starObjects)
        {
            StarInfo starInfo = starObject.GetComponent<StarInfo>();
            starObject.transform.position = new Vector3(starInfo.x0 * sliderValue, (starInfo.z0 + 1) * sliderValue, starInfo.y0 * sliderValue);
        }
        foreach (GameObject constellationLine in constellationLines)
        {
            StarFollower starFollower = constellationLine.GetComponent<StarFollower>();
            LineRenderer lineRenderer = constellationLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, starFollower.starToFollow1.transform.position);
            lineRenderer.SetPosition(1, starFollower.starToFollow2.transform.position);
        }
    }

    public void constellationDetails()
    {
        Debug.Log(currConstellation);
        if (currConstellation == 0)
        {
            noConstellationCanvas.gameObject.SetActive(true);
        } else if (currConstellation == 1)
        {
            modernCanvas.gameObject.SetActive(true);
        } else if (currConstellation == 2)
        {
            hawaiianCanvas.gameObject.SetActive(true);
        } else if (currConstellation == 3)
        {
            navajoCanvas.gameObject.SetActive(true);
        } else if (currConstellation == 4)
        {
            samoanCanvas.gameObject.SetActive(true);
        } else if (currConstellation == 5)
        {
            egyptianCanvas.gameObject.SetActive(true);
        }
    }
}