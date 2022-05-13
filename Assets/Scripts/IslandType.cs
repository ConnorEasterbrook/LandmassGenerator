using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandType : MonoBehaviour
{
    public Texture2D GetIslandInformation(int mapSize, int islandType)
    {
        float[,] heightMap = new float[mapSize, mapSize]; // Create a 2D array to represent each pixel

        // Get the height float value for each pixel
        heightMap = GetHeightMapFloat(heightMap, mapSize, islandType);

        Color[] colourMap = new Color[mapSize * mapSize]; // Place pixels into mapSized sized array for colour usage

        // Loop through every pixel in the map to get the pixel's current height
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                // Set each pixel to the correct black / white gradient based on it's position to display it's height value
                colourMap[x * mapSize + y] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return GetMapTexture(colourMap, mapSize);
    }

    public float[,] GetHeightMapFloat(float[,] heightMap, int mapSize, int islandType)
    {
        if (islandType == 0)
        {
            return heightMap = GenerateFalloffMapFloat(mapSize);
        }
        else
        {
            return heightMap;
        }
    }

    // This function serves to generate the height float value for each pixel in our map
    public float[,] GenerateFalloffMapFloat(int mapSize)
    {
        float[,] heightMap = new float[mapSize, mapSize]; // Create a 2D array to represent each pixel

        // Loop through each pixel and generate the desired height value of each one
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                float x = i / (float)mapSize * 2 - 1;
                float y = j / (float)mapSize * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)); // Get the absolute value of the current pixel's pos

                heightMap[i, j] = EvaluateFallOff(value);
            }
        }

        return heightMap;
    }

    // This is our base calculation to represent the shape of our desired falloffMap
    static float EvaluateFallOff(float value)
    {
        // Establish basic values for graph calculation
        float a = 3;
        float b = 2.2f;

        // This Mathf.Pow usage is basically the mathematics behind our falloffMap graph
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    private Texture2D GetMapTexture(Color[] colourMap, int mapSize)
    {
        Texture2D tex = new Texture2D(mapSize, mapSize); // Create a new texture and set it to be the same dimensions as our given image
        tex.filterMode = FilterMode.Point; // Stop colour blurring so there's no colour overlapping given region borders
        tex.wrapMode = TextureWrapMode.Clamp; // Stop possible instances of colour leaking over from one edge to the other
        tex.SetPixels(colourMap); // Set our new texture's pixels to the given colours
        tex.Apply(); // Apply the changes

        return tex;
    }
}
