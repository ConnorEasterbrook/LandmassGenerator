using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMap : MonoBehaviour
{
    //public GameObject cubePrefab; // The voxel prefab with the voxel generation scripts attached
    GameObject cubeParent; // Intended parent object of voxels
    GameObject combinedObj;

    Material voxelMat;

    public void EstablishVoxels(int mapSize, GameObject mapPlane, float[,] noiseMap, TerrainType[] regions, bool cubeHeight)
    {
        cubeParent = new GameObject ("CubeParent"); // Establish an empty gameObject to use as a parent for generated cubes
        cubeParent.transform.SetParent (transform); // Set voxel parent to be the child of the object containing this script

        Vector3 voxelSize = new Vector3 ((mapPlane.transform.localScale.x * 10) / mapSize, 2f, (mapPlane.transform.localScale.z * 10) / mapSize); // Set the desired voxel size

        // Voxel grid variables
        float mapOffset = (float) mapSize; // A weird calculation I found that centers the voxel map for us
        Vector3 originPosition = new Vector3 (transform.position.x - 30, transform.position.y, transform.position.z - 30); // Establish where our grid will begin

        Color[] colourMap = new Color [mapSize * mapSize]; // Place voxels into map sized array for colour usage 

        GameObject[] voxels = new GameObject [mapSize * mapSize];

        int index = -1; // Our voxel count

        // Loop through each voxel and increase our index for each one
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                index++;

                voxelMat = new Material (Shader.Find ("Standard")); // Add a base material

                voxels [index] = GameObject.CreatePrimitive (PrimitiveType.Cube); // Create cube GameObject at Voxel location (SHOULD LATER BE REPLACED WITH PROCEDURAL CUBE GENERATION)

                // Establish individual voxel position
                originPosition = this.transform.position - new Vector3 ((float) 15 - ((float) voxelSize.x / 2), 0, (float) 15 - ((float) voxelSize.z / 2)); // Set to this instance of voxel
                originPosition.x += x * voxelSize.x; // Move voxel origin position to correct coordinate
                originPosition.z += z * voxelSize.z; // Move voxel origin position to correct coordinate
                // Establish individual voxel y position based on user's preferences
                if (cubeHeight)
                {
                    // Restrict water voxels from gaining height. Must change to block any visible holes in 3D shape //
                    //if (noiseMap [x, z] <= 0.25)
                    //{
                    //    originPosition.y = 0;
                    //}
                    //else
                    //{
                        originPosition.y = Mathf.RoundToInt (noiseMap [x, z] * 5); // Set y value of voxel to be rounded and low-res (Minceraft-like)
                    //}
                }
                else
                {
                    originPosition.y = noiseMap [x, z] * 5; // Set y value of voxel to be free and high-res
                }

                voxels [index].transform.position = originPosition;
                voxels [index].transform.localScale = voxelSize;
                voxels [index].transform.parent = cubeParent.transform;

                // Establish Voxel colouring
                float currentHeight = noiseMap [x, z];

                // Loop through each perlinRegion to set the pixel colour in the colour array
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions [i].height)
                    {
                        colourMap [x * mapSize + z] = regions [i].colour;

                        voxelMat.color = regions [i].colour; // Set our voxel material colour to match the perlin region it's in
                        voxelMat.name = regions [i].name; // Set our voxel material's name to match the given region

                        // If region is same height as our set water region then keep it at y 0 
                        if (regions [i].height < 0.25)
                        {
                            originPosition.y = 0;
                        }

                        break; // Once this is done we can break out of this loop
                    }
                }

                voxels [index].GetComponent <MeshRenderer>().material = voxelMat; // Set the voxel texture to the correct region
            }
        }

        MeshLoad (voxels, voxelMat, regions);
    }

    private void MeshLoad (GameObject[] voxels, Material voxelMat, TerrainType[] regions)
    {
        // Lists that hold mesh data belonging to the submesh depending on the region
        List <CombineInstance> deepWaterList    = new List <CombineInstance>();
        List <CombineInstance> shallowWaterList = new List <CombineInstance>();
        List <CombineInstance> sandList         = new List <CombineInstance>();
        List <CombineInstance> lowGrassList     = new List <CombineInstance>();
        List <CombineInstance> highGrassList    = new List <CombineInstance>();
        List <CombineInstance> stoneList        = new List <CombineInstance>();
        List <CombineInstance> mountainList     = new List <CombineInstance>();
        List <CombineInstance> mountainTipList  = new List <CombineInstance>();

        // Gameobjects created to house the voxels based on regions
        GameObject deepWaterRegion      = new GameObject ("deepWaterRegion");
        GameObject shallowWaterRegion   = new GameObject ("shallowWaterRegion");
        GameObject sandRegion           = new GameObject ("sandRegion");
        GameObject lowGrassRegion       = new GameObject ("lowGrassRegion");
        GameObject highGrassRegion      = new GameObject ("highGrassRegion");
        GameObject stoneRegion          = new GameObject ("stoneRegion");
        GameObject mountainRegion       = new GameObject ("mountainRegion");
        GameObject mountainTipRegion    = new GameObject ("mountainTipRegion");

        // Set the correct transform for the region parents
        deepWaterRegion     .transform.parent = cubeParent.transform;
        shallowWaterRegion  .transform.parent = cubeParent.transform;
        sandRegion          .transform.parent = cubeParent.transform;
        lowGrassRegion      .transform.parent = cubeParent.transform;
        highGrassRegion     .transform.parent = cubeParent.transform;
        stoneRegion         .transform.parent = cubeParent.transform;
        mountainRegion      .transform.parent = cubeParent.transform;
        mountainTipRegion   .transform.parent = cubeParent.transform;

        // Add the needed components to the region parents
        deepWaterRegion     .AddComponent <MeshFilter>();
        deepWaterRegion     .AddComponent <MeshRenderer>();
        shallowWaterRegion  .AddComponent <MeshFilter>();
        shallowWaterRegion  .AddComponent <MeshRenderer>();
        sandRegion          .AddComponent <MeshFilter>();
        sandRegion          .AddComponent <MeshRenderer>();
        lowGrassRegion      .AddComponent <MeshFilter>();
        lowGrassRegion      .AddComponent <MeshRenderer>();
        highGrassRegion     .AddComponent <MeshFilter>();
        highGrassRegion     .AddComponent <MeshRenderer>();
        stoneRegion         .AddComponent <MeshFilter>();
        stoneRegion         .AddComponent <MeshRenderer>();
        mountainRegion      .AddComponent <MeshFilter>();
        mountainRegion      .AddComponent <MeshRenderer>();
        mountainTipRegion   .AddComponent <MeshFilter>();
        mountainTipRegion   .AddComponent <MeshRenderer>();

        for (int i = 0; i < voxels.Length; i++)
        {
            voxels [i].SetActive (false); // Hide the current cube

            MeshFilter meshFilter = voxels [i].GetComponent <MeshFilter>(); // Make a new meshFilter and give it the current voxel's

            // Get the voxel material name for sorting purposes
            MeshRenderer meshRenderer = voxels [i].GetComponent <MeshRenderer>();
            string materialName = meshRenderer.material.name;

            CombineInstance combine = new CombineInstance(); // Create a new Combine Instance
            
            // Find which list the cube belongs in
            if (materialName == "Deep Water (Instance)")
            {
                voxels [i].transform.parent = deepWaterRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                deepWaterList.Add (combine); // Finally, add our mesh to the matching list
            }
            else if (materialName == "Water (Instance)")
            {
                voxels [i].transform.parent = shallowWaterRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                shallowWaterList.Add (combine); // Finally, add our mesh to the matching list
            }
            else if (materialName == "Sand (Instance)")
            {
                voxels [i].transform.parent = sandRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                sandList.Add (combine); // Finally, add our mesh to the matching list
            }
            else if (materialName == "Low Land (Instance)")
            {
                voxels [i].transform.parent = lowGrassRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                lowGrassList.Add (combine); // Finally, add our mesh to the matching list
            }
            else if (materialName == "High Land (Instance)")
            {
                voxels [i].transform.parent = highGrassRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                highGrassList.Add (combine); // Finally, add our mesh to the matching list
            }
            else if (materialName == "Stone (Instance)")
            {
                voxels [i].transform.parent = stoneRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                stoneList.Add (combine); // Finally, add our mesh to the matching list
            }
            else if (materialName == "Mountain (Instance)")
            {
                voxels [i].transform.parent = mountainRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                mountainList.Add (combine); // Finally, add our mesh to the matching list
            }
            else if (materialName == "Mountain Tip (Instance)")
            {
                voxels [i].transform.parent = mountainTipRegion.transform;

                combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                mountainTipList.Add (combine); // Finally, add our mesh to the matching list
            }
        }
        
        // Combine the region type into a single mesh
        Mesh combinedDeepWaterMesh = new Mesh(); // Make it into a new mesh
        combinedDeepWaterMesh.CombineMeshes (deepWaterList.ToArray()); // Add all meshes in list to array
        Mesh combinedShallowWaterMesh = new Mesh(); // Make it into a new mesh
        combinedShallowWaterMesh.CombineMeshes (shallowWaterList.ToArray()); // Add all meshes in list to array
        Mesh combinedSandMesh = new Mesh(); // Make it into a new mesh
        combinedSandMesh.CombineMeshes (sandList.ToArray()); // Add all meshes in list to array
        Mesh combinedLowGrassMesh = new Mesh(); // Make it into a new mesh
        combinedLowGrassMesh.CombineMeshes (lowGrassList.ToArray()); // Add all meshes in list to array
        Mesh combinedHighGrassMesh = new Mesh(); // Make it into a new mesh
        combinedHighGrassMesh.CombineMeshes (highGrassList.ToArray()); // Add all meshes in list to array
        Mesh combinedStoneMesh = new Mesh(); // Make it into a new mesh
        combinedStoneMesh.CombineMeshes (stoneList.ToArray()); // Add all meshes in list to array
        Mesh combinedMountainMesh = new Mesh(); // Make it into a new mesh
        combinedMountainMesh.CombineMeshes (mountainList.ToArray()); // Add all meshes in list to array
        Mesh combinedMountainTipMesh = new Mesh(); // Make it into a new mesh
        combinedMountainTipMesh.CombineMeshes (mountainTipList.ToArray()); // Add all meshes in list to array

        // Set our region parents to be the correct mesh filter
        deepWaterRegion     .GetComponent <MeshFilter>().mesh = combinedDeepWaterMesh;
        shallowWaterRegion  .GetComponent <MeshFilter>().mesh = combinedShallowWaterMesh;
        sandRegion          .GetComponent <MeshFilter>().mesh = combinedSandMesh;
        lowGrassRegion      .GetComponent <MeshFilter>().mesh = combinedLowGrassMesh;
        highGrassRegion     .GetComponent <MeshFilter>().mesh = combinedHighGrassMesh;
        stoneRegion         .GetComponent <MeshFilter>().mesh = combinedStoneMesh;
        mountainRegion      .GetComponent <MeshFilter>().mesh = combinedMountainMesh;
        mountainTipRegion   .GetComponent <MeshFilter>().mesh = combinedMountainTipMesh;

        // Set our region parent to have a material that hasn't been influenced
        deepWaterRegion     .GetComponent <MeshRenderer>().material = voxelMat;
        shallowWaterRegion  .GetComponent <MeshRenderer>().material = voxelMat;
        sandRegion          .GetComponent <MeshRenderer>().material = voxelMat;
        lowGrassRegion      .GetComponent <MeshRenderer>().material = voxelMat;
        highGrassRegion     .GetComponent <MeshRenderer>().material = voxelMat;
        stoneRegion         .GetComponent <MeshRenderer>().material = voxelMat;
        mountainRegion      .GetComponent <MeshRenderer>().material = voxelMat;
        mountainTipRegion   .GetComponent <MeshRenderer>().material = voxelMat;

        // Get our region parents to copy the material from the given cubes
        deepWaterRegion     .GetComponent <MeshRenderer>().material.color = regions [0].colour;
        shallowWaterRegion  .GetComponent <MeshRenderer>().material.color = regions [1].colour;
        sandRegion          .GetComponent <MeshRenderer>().material.color = regions [2].colour;
        lowGrassRegion      .GetComponent <MeshRenderer>().material.color = regions [3].colour;
        highGrassRegion     .GetComponent <MeshRenderer>().material.color = regions [4].colour;
        stoneRegion         .GetComponent <MeshRenderer>().material.color = regions [5].colour;
        mountainRegion      .GetComponent <MeshRenderer>().material.color = regions [6].colour;
        mountainTipRegion   .GetComponent <MeshRenderer>().material.color = regions [7].colour;
    }
}
