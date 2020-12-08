using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// - Goatbandit

// Intended as a kind of 'game manager' script, MapDisplay allows us to control all variables related to displaying the desired map(s)
public class MapDisplay : MonoBehaviour
{
    [Header ("Display Settings")]
    // Establish the display we'll be putting our texture on
    [Min (1)] public int mapSize; // Set dimensions of the image
    public GameObject displayPlane; // Desired GameObject to display textures on
    private Renderer textureRenderer; // Map specific renderer
    private MeshFilter meshFilter; // Selected gameObjects Mesh Filter
    private MeshRenderer meshRenderer; // Selected gameObjects Mesh Renderer

    [Header ("Voronoi Settings")]
    public int voronoiRegionAmount; // Set the amount of regions within the dimensions

    [Header ("Perlin Noise Settings")]
    [Min (2)] public float noiseScale; // The scale of the perlin noise on our map
    [Range (1, 8)] public int octaves; // How fine the detail is on the map
    [Range (0, 2)] public float persistance; // The fullness of the terrain
    [Range (1, 2)] public float lacunarity; // Aggressiveness of the noise details
    public int seed; // The seed used for generation (0 means random)
    public Vector2 offset; // Move around the noise map in the editor
    
    // Perlin Colour Parameters
    public TerrainType[] perlinRegions;
    private float[,] noiseMap;
    private Color[] colourMap;

    [Header ("Falloff Settings")]
    public bool useFalloff;
    private float[,] falloffMap;

    [Header ("3D Settings")]
    public bool threeDimensional;
    public Material cubeMat; // Set the base cube material

    // Set the map display to be your desired map
    public enum DrawMode
    {
        VoronoiColourMap,
        VoronoiDistanceMap,
        FalloffMap,
        PerlinNoiseMap,
        ColourPerlinNoiseMap
    };
    public DrawMode drawMode; // Allow us to change our DrawMode enum in inspector

    // Create references to desired scripts
    private Voronoi voronoi;
    private PerlinNoise perlinNoise;
    private PerlinColour perlinColour;
    private FalloffGenerator falloff;
    private VoxelMap voxelMap;

    public bool autoUpdate; // Choose whether we want to see inspector changes in real-time

    void Awake()
    {
        Generate();
    }

    // This function serves as our real time generator for both in and out of gameplay
    public void Generate()
    {
        // If the seed is set to 0 then we should randomise it
        if (seed == 0)
        {
            seed = new System.Random().Next();
        }

        // Delete children in Generate() to avoid duplicates in editor
        foreach (Transform child in transform)
        {
            DestroyImmediate (child.gameObject);
        }

        DrawMapDisplay();
    }

    void DrawMapDisplay()
    {
        // Establish our script references
        voronoi         = GetComponent <Voronoi>(); 
        perlinNoise     = GetComponent <PerlinNoise>();
        perlinColour    = GetComponent <PerlinColour>();
        falloff         = GetComponent <FalloffGenerator>();
        voxelMap        = GetComponent <VoxelMap>();
        
        noiseMap = perlinNoise.GenerateNoiseMap (mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset); // Establish the noiseMap values for our perlin options
        falloffMap = falloff.GenerateFalloffMapFloat (mapSize); // Get the desired falloff map values that will affect terrain generation

        if (useFalloff)
        {
            // Loop through every pixel within the noiseMap and clamp it's value to the corresponding falloffMap value
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    noiseMap [x, y] = Mathf.Clamp01 (noiseMap [x, y] - falloffMap [x, y]); // The clamp ensures that the white areas of falloffMap don't ruin our terrain
                }
            }
        }
        
        // If threeDimensional = true then create a 3D representation of the rendered map. If false then destroy the 3D objects generated
        if (threeDimensional)
        {
            // pColourMesh.ColourCubeSpawner (mapSize, noiseMap, perlinRegions, cubeMat);
            voxelMap.EstablishVoxels (mapSize, displayPlane, noiseMap, perlinRegions);
        }
        else if (transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate (child.gameObject);
            }
        }

        // Establish required mesh components
        meshFilter      = displayPlane.GetComponent <MeshFilter>();
        meshRenderer    = displayPlane.GetComponent <MeshRenderer>();

        // Select what is displayed by our selected enum by calling the inital function in the desired script. Carrying over dimensions and desired variables for texture generation
        // and bringing back the returned texture2D to DrawMap(), where we set the object material to the new texture
        if (drawMode == DrawMode.VoronoiColourMap)
        {
            DrawMap (voronoi.GetColourImageTexture (mapSize, voronoiRegionAmount));
        }
        else if (drawMode == DrawMode.VoronoiDistanceMap)
        {
            DrawMap (voronoi.GetCellularNoiseImageTexture (mapSize, voronoiRegionAmount));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            DrawMap (falloff.GenerateFalloffMap (mapSize));
        }
        else if (drawMode == DrawMode.PerlinNoiseMap)
        {
            DrawMap (perlinNoise.GenerateNoiseTexture (mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset));
        }
        else if (drawMode == DrawMode.ColourPerlinNoiseMap)
        {
            DrawMap (perlinColour.GenerateMap (mapSize, noiseMap, perlinRegions));
        }
    }

    // Set the displayPlane material to our given texture
    // texture - The returned variable from calling this function with another from a different script
    void DrawMap (Texture2D texture)
    {
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}

// Store the parameters that dictate our terrain
[System.Serializable]
public struct TerrainType
{
    [Tooltip ("Name this terrain")]
    public string name; // Parameter for us to set the desired name of the terrain level

    [Tooltip ("Set the beginning height")]
    public float height; // Parameter for us to set the desired height of the terrain

    [Tooltip ("Designate a colour")]
    public Color colour; // Paramater for us to set the desired colour of the terrain
}

// - Goatbandit
