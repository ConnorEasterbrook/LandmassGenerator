using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create and display a voronoi diagram
public class Voronoi : MonoBehaviour
{
    // Generate a texture for our voronoi diagram by generating a random colour for each region, finding and colouring the pixels within that region to the given region colour, and then generating it into a new texture. Once fully executed, we return the created image texture with the designated pixel colours.
    public Texture2D GetColourImageTexture(int mapSize, int regionAmount)
    {
        Vector2Int[] polygonPoint = new Vector2Int [regionAmount]; // The points that'll be used to divide the area with
        Color[] regions = new Color [regionAmount]; // To see our regions more clearly, it would be nice to give them a colour

        // For every region we should place a random point and give the point's region a random colour
        for (int i = 0; i < regionAmount; i++)
        {
            polygonPoint [i] = new Vector2Int (Random.Range (0, mapSize), Random.Range (0, mapSize)); // Place polygonPoint at a random location between .zero and our max dimension
            regions [i] = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 1f); // Random RGB values with an opacity of 100%
        }

        Color[] pixelColours = new Color [mapSize * mapSize]; // Place pixels into image sized array for colour usage 

        // Loop through each pixel to find it's correct region colour based on how close it is to a given polygonPoint
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                int index = x * mapSize + y; // Find correct position within colour index

                pixelColours [index] = regions [GetClosestPolygonPointIndex (new Vector2Int (x, y), polygonPoint)]; // Call function to find closest polygonPoint to designate correct colour to region
            }
        }

        return GetImageFromColourArray (pixelColours, mapSize);
    }

    // Generate a noise texture for our voronoi diagram by generating a colour for each pixel based on its proximity to a polygonPoint, and then generating it into a new texture. 
    public Texture2D GetCellularNoiseImageTexture(int mapSize, int regionAmount)
    {
        Vector2Int[] polygonPoint = new Vector2Int [regionAmount]; // The points that'll be used to divide the area with

        // For every region we should place a random point
        for (int i = 0; i < regionAmount; i++)
        {
            polygonPoint [i] = new Vector2Int (Random.Range (0, mapSize), Random.Range (0, mapSize)); // Place polygonPoint at a random location between .zero and our max dimension
        }

        Color[] pixelColours = new Color [mapSize * mapSize]; // Place pixels into image sized array for colour usage 
        float[] distances = new float [mapSize * mapSize]; // Place pixels into image sized array for calculations

        // Loop through each pixel to find it's correct region based on how close it is to a given polygonPoint
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                int index = x * mapSize + y; // Find correct position within distances index

                // Call function to find closest polygonPoint to designate correct colour to region
                distances [index] = Vector2.Distance (new Vector2Int (x, y), polygonPoint [GetClosestPolygonPointIndex (new Vector2Int (x, y), polygonPoint)]);
            }
        }

        float maxDistance = GetMaxDistance (distances);

        // Loop through each pixel to establish it's given colour based on its distance from closest polygonPoint
        for (int i = 0; i < distances.Length; i++)
        {
            float colourValue = distances [i] / maxDistance;
            pixelColours [i] = new Color (colourValue, colourValue, colourValue, 1f); // Set the pixel colour to be distance colour at 100% opacity
        }

        return GetImageFromColourArray (pixelColours, mapSize);
    }

    // This function serves to use the distance from a pixel to a polygonPoint to generate a colour value based on that distance
    float GetMaxDistance (float[] distances)
    {
        float maxDistance = float.MinValue;

        // Loop through each pixel given to establish it's distance from a polygonPoint into a float value
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances [i] > maxDistance)
            {
                maxDistance = distances [i];
            }
        }

        return maxDistance; // Return the converted singular float value
    }

    // This function serves to find the closest polygonPoint to a given pixel
    int GetClosestPolygonPointIndex (Vector2Int pixelPos, Vector2Int[] polygonPoint)
    {
        float smallestDistance = float.MaxValue;
        int index = 0;

        // Go through each polygonPoint within the array and find the smallest distance between it and the pixelPos
        for (int i = 0; i < polygonPoint.Length; i++)
        {
            // If a pixel does not have the correct smallest distance applied then correct it
            if (Vector2.Distance (pixelPos, polygonPoint [i]) < smallestDistance)
            {
                smallestDistance = Vector2.Distance (pixelPos, polygonPoint [i]); // Set the closest polygonPoint as correct one
                index = i;
            }
        }

        return index;
    }

    // This function serves to place the given colours onto our image texture
    private Texture2D GetImageFromColourArray (Color[] pixelColours, int mapSize)
    {
        Texture2D tex = new Texture2D (mapSize, mapSize); // Create a new texture and set it to be the same dimensions as our given image
        tex.filterMode = FilterMode.Point; // Stop colour blurring so there's no colour overlapping given region borders
        tex.wrapMode = TextureWrapMode.Clamp; // Stop possible instances of colour leaking over from one edge to the other
        tex.SetPixels (pixelColours); // Set our new texture's pixels to the given colours
        tex.Apply(); // Apply the changes

        return tex; 
    }
}