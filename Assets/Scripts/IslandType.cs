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
        else if (islandType == 2)
        {
            return heightMap = GenerateRoundMapFloat(mapSize);
        }
        else if (islandType == 3)
        {
            return heightMap;
        }
        else if (islandType == 4)
        {
            return heightMap = GenerateLakeMapFloat(mapSize);
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
                // Position of the gradient
                float x = i / (float)mapSize * 2 - 1;
                float y = j / (float)mapSize * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)); // Limit the edges of the gradient

                heightMap[i, j] = GradientConstraints(value, 3, 2.2f);
            }
        }

        return heightMap;
    }

    // This function serves to generate the height float value for each pixel in our map
    public float[,] GenerateRoundMapFloat(int mapSize)
    {
        float[,] heightMap = new float[mapSize, mapSize]; // Create a 2D array to represent each pixel
        Vector2 centerPoint = new Vector2(mapSize / 2, mapSize / 2);

        // Loop through each pixel and generate the desired height value of each one
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                float distanceFromCenter = Vector2.Distance(centerPoint, new Vector2(x, y));
                float pixelHeight = 1;

                if ((1 - (distanceFromCenter / mapSize)) >= 0)
                {
                    pixelHeight = (0.375f + (distanceFromCenter / mapSize));
                }
                else
                {
                    pixelHeight = 0;
                }

                heightMap[x, y] = GradientConstraints(pixelHeight, 3, 3f);
            }
        }

        return heightMap;
    }

    // This function serves to generate the height float value for each pixel in our map
    public float[,] GenerateLakeMapFloat(int mapSize)
    {
        float[,] heightMap = new float[mapSize, mapSize]; // Create a 2D array to represent each pixel
        Vector2 centerPoint = new Vector2(mapSize / 2, mapSize / 2);

        // Loop through each pixel and generate the desired height value of each one
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float distanceFromCenter = Vector2.Distance(centerPoint, new Vector2(x, y));
                float pixelHeight = 1;

                if ((1 - (distanceFromCenter / mapSize)) >= 0)
                {
                    pixelHeight = (1 - (distanceFromCenter / mapSize));
                }
                else
                {
                    pixelHeight = 0;
                }

                heightMap[x, y] = GradientConstraints(pixelHeight, 4f, 3.5f);
            }
        }

        return heightMap;
    }

    // This is our base calculation to represent the shape of our desired falloffMap
    static float GradientConstraints(float value, float density, float size)
    {
        // Establish basic values for graph calculation
        float gradientDensity = density;
        float gradientSize = size;

        // This Mathf.Pow usage is basically the mathematics behind our falloffMap graph
        return Mathf.Pow(value, gradientDensity) / (Mathf.Pow(value, gradientDensity) + Mathf.Pow(gradientSize - gradientSize * value, gradientDensity));
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
