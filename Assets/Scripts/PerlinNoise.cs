using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generate and render a perlin noise value onto a texture for MapDisplay.cs
public class PerlinNoise : MonoBehaviour
{
    // Create and setup the perlin noise values that will make up the map display
    public Texture2D GenerateNoiseTexture (int mapSize, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float [mapSize, mapSize]; // Set up the map values that will eventually be put into a texture

        // Noise parameters
        System.Random prng = new System.Random (seed); // Noise generation seed
        Vector2[] octaveOffsets = new Vector2 [octaves]; // Prepare octaves for sampling

        // Sample our octaves with seed-based positioning. Essentially allows you to use a seed to get the desired noise map whenever
        for (int i = 0; i < octaves; i++) 
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y; // Invert yOffset to make map offset changes work as intended

            octaveOffsets[i] = new Vector2 (offsetX, offsetY); // Assign each offset in the array to the coordinate from the seed
        }
           

        // Keep track of the highest and lowest values within our noiseMap array
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        // Get the mid-point of the map so that changes are centered instead of anchored to the top right of the map
        float halfMap = mapSize / 2f;

        // Loop through each pixel to generate a value for it
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                // Establish some parameters that effect our perlin noise texture to give it more of a realistic look
                float frequency = 1; // Distance between sample points (further apart = more rapid height changes)
                float amplitude = 1; // The strength of the effect of frequency on octaves
                float noiseHeight = 0;

                // For each octave, loop through our pixel coords. This dictates the fineness of the detail within the map
                for (int i = 0; i < octaves; i++)
                {
                    // Generate the height value of each pixel.                                  //   Divide by scale so that we don't get the same value every time.
                    float sampleX = (x - halfMap + octaveOffsets[i].x) / noiseScale * frequency; // + octaveOffsets is because we want each octave to be sampled from the specified seed location
                    float sampleY = (y - halfMap + octaveOffsets[i].y) / noiseScale * frequency; // - halfMap means changes will anchor to the center rather than top right of our map

                    // Create the perlin value of each pixel using the pixel values. We can get more interesting noise by allowing the perlin value to be negative (* 2 - 1)
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; 

                    noiseMap [x, y] = perlinValue; // Finally update the specified pixel in our array to match the perlin value given

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance; // Amplitude multiplied by the fullness of the terrain
                    frequency *= lacunarity; // Distance between sample points multiplied by aggresiveness of details
                }

                // Set range of noiseHeight limits and ensure that our noiseHeight value doesn't go out of range
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap [x, y] = noiseHeight; // Set our 2D float array to the terrain height of coord position
            }
        }

        // Loop through noiseMap values again and set each coord noise value to the InverseLerp result
            for(int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    // If noiseMap value = minNoise then it will return 0. = maxNoise then 1. If halfway then 0.5. Best for single map (not endless)
                    noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]); 
                }
            }

        return DrawNoiseMap (noiseMap, mapSize); // Finally return the pixel values for texture generation
    }

    // This function serves to create the NoiseMap texture from the perlin noise values
    private Texture2D DrawNoiseMap (float[,] noiseMap, int mapSize)
    {
        Color[] pixelColours = new Color [mapSize * mapSize]; // Place pixels into image sized array for colour usage 

        // Loop through each pixel to find it's correct perlin colour based on its given height
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                pixelColours [x * mapSize + y] = Color.Lerp (Color.black, Color.white, noiseMap [x, y]);
            }
        }

        Texture2D tex = new Texture2D (mapSize, mapSize); // Create a new texture and set it to be the same dimensions as our given image
        tex.filterMode = FilterMode.Point; // Stop colour blurring so there's no colour overlapping given region borders
        tex.wrapMode = TextureWrapMode.Clamp; // Stop possible instances of colour leaking over from one edge to the other
        tex.SetPixels (pixelColours); // Set our new texture's pixels to the given colours
        tex.Apply(); // Apply the changes

        return tex;
    }

    // Create and setup the noiseMap values that will make up the colour map display
    public float[,] GenerateNoiseMap (int mapSize, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float [mapSize, mapSize]; // Set up the map values that will eventually be put into a texture

        // Noise parameters
        System.Random prng = new System.Random (seed); // Noise generation seed
        Vector2[] octaveOffsets = new Vector2 [octaves]; // Prepare octaves for sampling

        // Sample our octaves with seed-based positioning. Essentially allows you to use a seed to get the desired noise map whenever
        for (int i = 0; i < octaves; i++) 
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y; // Invert yOffset to make map offset changes work as intended

            octaveOffsets[i] = new Vector2 (offsetX, offsetY); // Assign each offset in the array to the coordinate from the seed
        }
           

        // Keep track of the highest and lowest values within our noiseMap array
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        // Get the mid-point of the map so that changes are centered instead of anchored to the top right of the map
        float halfMap = mapSize / 2f;

        // Loop through each pixel to generate a value for it
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                // Establish some parameters that effect our perlin noise texture to give it more of a realistic look
                float frequency = 1; // Distance between sample points (further apart = more rapid height changes)
                float amplitude = 1; // The strength of the effect of frequency on octaves
                float noiseHeight = 0;

                // For each octave, loop through our pixel coords. This dictates the fineness of the detail within the map
                for (int i = 0; i < octaves; i++)
                {
                    // Generate the height value of each pixel.                                  //   Divide by scale so that we don't get the same value every time.
                    float sampleX = ((x - halfMap) / noiseScale + octaveOffsets[i].x) * frequency; // + octaveOffsets is because we want each octave to be sampled from the specified seed location
                    float sampleY = ((y - halfMap) / noiseScale + octaveOffsets[i].y) * frequency; // - halfMap means changes will anchor to the center rather than top right of our map

                    // Create the perlin value of each pixel using the pixel values. We can get more interesting noise by allowing the perlin value to be negative (* 2 - 1)
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; 

                    noiseMap [x, y] = perlinValue; // Finally update the specified pixel in our array to match the perlin value given

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance; // Amplitude multiplied by the fullness of the terrain
                    frequency *= lacunarity; // Distance between sample points multiplied by aggresiveness of details
                }

                // Set range of noiseHeight limits and ensure that our noiseHeight value doesn't go out of range
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap [x, y] = noiseHeight; // Set our 2D float array to the terrain height of coord position
            }
        }

        // Loop through noiseMap values again and set each coord noise value to the InverseLerp result
            for(int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    // If noiseMap value = minNoise then it will return 0. = maxNoise then 1. If halfway then 0.5. Best for single map (not endless)
                    noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]); 
                }
            }

        return noiseMap; // Finally return the pixel values for colour generation
    }
}
