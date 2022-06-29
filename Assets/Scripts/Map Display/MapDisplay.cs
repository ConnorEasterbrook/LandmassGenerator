using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Intended as a kind of 'game manager' script, MapDisplay allows us to control all variables related to displaying the desired map(s)
public class MapDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    // Establish the display we'll be putting our texture on
    [Min(1)] public int mapSize; // Set dimensions of the image
    public GameObject displayPlane; // Desired GameObject to display textures on
    private MeshRenderer meshRenderer; // Selected gameObjects Mesh Renderer

    [Header("Perlin Noise Settings")]
    [Range(50, 200)] public float zoom; // The scale of the perlin noise on our map
    [Range(2, 8)] public int levelOfDetail; // How fine the detail is on the map
    [Range(0.5f, 2)] public float amplitude; // The fullness of the terrain
    [Range(0.5f, 2)] public float frequency; // Aggressiveness of the noise details
    public int seed; // The seed used for generation (0 means random)
    public Vector2 offset; // Move around the noise map in the editor

    // Perlin Colour Parameters
    public TerrainType[] perlinRegions;
    private float[,] noiseMap;
    private Color[] colourMap;

    [Header("Island Settings")]
    public IslandTypeEnum islandType;
    public enum IslandTypeEnum
    {
        Regular,
        Pangaea,
        Round,
        Ring,
        Archipelago,
        Lake
    };
    private int islandTypeIdentifier;
    private float[,] falloffMap;

    [Header("Voronoi Settings")]
    public int voronoiRegionAmount; // Set the amount of regions within the dimensions

    // Set the map display to be your desired map
    public enum DrawMode
    {
        VoronoiColourMap,
        VoronoiDistanceMap,
        ShowIslandMap,
        PerlinNoiseMap,
        ColourPerlinNoiseMap
    };
    [Header("Map Settings")]
    public DrawMode drawMode; // Allow us to change our DrawMode enum in inspector

    // Create references to desired scripts
    public Voronoi voronoi;
    public PerlinNoise perlinNoise;
    public PerlinColour perlinColour;
    public IslandType islandTypeScript;

    [Header("Inspector Tools")]
    public bool autoUpdate; // Choose whether we want to see inspector changes in real-time

    void Awake()
    {
        GetReferences();
        Generate();
    }

    private void Start()
    {
        autoUpdate = false;
    }

    public void GetReferences()
    {
        // Establish required mesh components
        meshRenderer = displayPlane.GetComponent<MeshRenderer>();
    }

    // This function serves as our real time generator for both in and out of gameplay
    public void Generate()
    {
        // If the seed is set to 0 then we should randomise it
        if (seed == 0)
        {
            seed = new System.Random().Next();
        }

        // Delete children in 3D Transform to avoid duplicates
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        DrawMapDisplay();
    }

    void DrawMapDisplay()
    {
        noiseMap = perlinNoise.GenerateNoiseMap(mapSize, seed, zoom, levelOfDetail, amplitude, frequency, offset); // Establish the noiseMap values for our perlin options
        CheckSettings();

        // Display the raw version of generated maps via DrawMode, calling the relevant script and function to generate a Texture2D
        if (drawMode == DrawMode.VoronoiColourMap)
        {
            DrawMap(voronoi.GetColourImageTexture(mapSize, voronoiRegionAmount));
        }
        else if (drawMode == DrawMode.VoronoiDistanceMap)
        {
            DrawMap(voronoi.GetCellularNoiseImageTexture(mapSize, voronoiRegionAmount));
        }
        else if (drawMode == DrawMode.ShowIslandMap)
        {
            DrawMap(islandTypeScript.GetIslandInformation(mapSize, islandTypeIdentifier));
        }
        else if (drawMode == DrawMode.PerlinNoiseMap)
        {
            DrawMap(perlinNoise.GenerateNoiseTexture(mapSize, seed, zoom, levelOfDetail, amplitude, frequency, offset));
        }
        else if (drawMode == DrawMode.ColourPerlinNoiseMap)
        {
            DrawMap(perlinColour.GenerateMap(mapSize, noiseMap, perlinRegions));
        }
    }

    // Set the displayPlane material to our given texture
    void DrawMap(Texture2D texture)
    {
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    void CheckSettings()
    {
        if (drawMode != DrawMode.ShowIslandMap || drawMode != DrawMode.ColourPerlinNoiseMap)
        {
            GetIslandType();
        }
    }

    void GetIslandType()
    {
        if (islandType == IslandTypeEnum.Regular)
        {
            islandTypeIdentifier = 0;
            
            falloffMap = islandTypeScript.GenerateFalloffMapFloat(mapSize); // Get the desired falloff map values that will affect terrain generation

            // Loop through every pixel within the noiseMap and clamp it's value to the corresponding falloffMap value
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]); // The clamp ensures that the white areas of falloffMap don't ruin our terrain
                }
            }
        }
        else if (islandType == IslandTypeEnum.Pangaea)
        {
            islandTypeIdentifier = 1;
        }
        else if (islandType == IslandTypeEnum.Round)
        {
            islandTypeIdentifier = 2;
            
            falloffMap = islandTypeScript.GenerateRoundMapFloat(mapSize); // Get the desired falloff map values that will affect terrain generation

            // Loop through every pixel within the noiseMap and clamp it's value to the corresponding falloffMap value
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]); // The clamp ensures that the white areas of falloffMap don't ruin our terrain
                }
            }
        }
        else if (islandType == IslandTypeEnum.Ring)
        {
            islandTypeIdentifier = 3; 
        }
        else if (islandType == IslandTypeEnum.Archipelago)
        {
            islandTypeIdentifier = 4;
        }
        else if (islandType == IslandTypeEnum.Lake)
        {
            islandTypeIdentifier = 5;
            
            falloffMap = islandTypeScript.GenerateLakeMapFloat(mapSize); // Get the desired falloff map values that will affect terrain generation

            // Loop through every pixel within the noiseMap and clamp it's value to the corresponding falloffMap value
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]); // The clamp ensures that the white areas of falloffMap don't ruin our terrain
                }
            }
        }
    }
}

// Store the parameters that dictate our terrain
[System.Serializable]
public struct TerrainType
{
    [Tooltip("Name this terrain")]
    public string name; // Parameter for us to set the desired name of the terrain level

    [Tooltip("Set the beginning height")]
    public float height; // Parameter for us to set the desired height of the terrain

    [Tooltip("Designate a colour")]
    public Color colour; // Paramater for us to set the desired colour of the terrain
}
