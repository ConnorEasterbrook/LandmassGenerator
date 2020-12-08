using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// - Goatbandit

public class PerlinColour : MonoBehaviour
{
    // This function serves to set our pixels to the correct colour based on their height and the given region parameters
    // mapSize - The desired size of the map
    // noiseMap - The perlin noise pixel value array carried over from MapDisplay.cs
    // regions - The region parameter array carried over from MapDisplay.cs
    public Texture2D GenerateMap (int mapSize, float[,] noiseMap, TerrainType[] regions)
    {
        Color[] colourMap = new Color [mapSize * mapSize]; // Place pixels into image sized array for colour usage 

        // Loop through every pixel in the map to get the pixel's current height
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                float currentHeight = noiseMap [x, y];

                // Loop through each perlinRegion to set the pixel colour in the colour array
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions [i].height)
                    {
                        colourMap [x * mapSize + y] = regions [i].colour;

                        break; // Once this is done we can break out of this loop
                    }
                }
            }
        }

        return GetPerlinColourMapTexture (colourMap, mapSize);
    }

    // This function serves to place the given colours onto our image texture
    // colourMap - This is the array that holds the information on what colour our pixels should be based on their given region
    // mapSize - The image dimensions brought over from MapDisplay.cs that dictate the size of the generated texture
    private Texture2D GetPerlinColourMapTexture (Color[] colourMap, int mapSize)
    {
        Texture2D tex = new Texture2D (mapSize, mapSize); // Create a new texture and set it to be the same dimensions as our given image
        tex.filterMode = FilterMode.Point; // Stop colour blurring so there's no colour overlapping given region borders
        tex.wrapMode = TextureWrapMode.Clamp; // Stop possible instances of colour leaking over from one edge to the other
        tex.SetPixels (colourMap); // Set our new texture's pixels to the given colours
        tex.Apply(); // Apply the changes

        return tex;
    }
}

// - Goatbandit
