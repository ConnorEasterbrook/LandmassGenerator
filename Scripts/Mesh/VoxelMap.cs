using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMap : MonoBehaviour
{
    GameObject cubeParent; // Intended parent object of voxels

    Material voxelMat;

    public void EstablishVoxels(int mapSize, GameObject mapPlane, float[,] noiseMap, TerrainType[] regions)
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
                originPosition.y = noiseMap [x, z] * 5; // Set y value of voxel
                originPosition.x += x * voxelSize.x; // Move voxel origin position to correct coordinate
                originPosition.z += z * voxelSize.z; // Move voxel origin position to correct coordinate

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
    {/*
        // Lists that hold mesh data belonging to the submesh depending on the region
        List <CombineInstance> deepWaterList    = new List <CombineInstance>();
        List <CombineInstance> shallowWaterList = new List <CombineInstance>();
        List <CombineInstance> sandList         = new List <CombineInstance>();
        List <CombineInstance> lowGrassList     = new List <CombineInstance>();
        List <CombineInstance> highGrassList    = new List <CombineInstance>();
        List <CombineInstance> stoneList        = new List <CombineInstance>();
        List <CombineInstance> mountainList     = new List <CombineInstance>();
        List <CombineInstance> mountainTipList  = new List <CombineInstance>();

        // If the parent gameObject has no mesh filter then we should give it one
        if (cubeParent.gameObject.GetComponent <MeshFilter>() == null)
        {
            cubeParent.transform.gameObject.AddComponent <MeshFilter>(); // Add a new meshFilter to our parent object
        }

        // Add a MeshRenderer to our parent object if there isn't one
        if (cubeParent.GetComponent <MeshRenderer>() == null)
        {
            cubeParent.AddComponent <MeshRenderer>();
            MeshRenderer parentRenderer = cubeParent.GetComponent <MeshRenderer>();
        }*/

        // Establish our arrays for mesh combination
        MeshFilter[] meshes = GetComponentsInChildren <MeshFilter>();
        CombineInstance[] combined = new CombineInstance [meshes.Length];

        for (int i = 0; i < voxels.Length; i++)
        {
            GameObject currentChunk = new GameObject();
            currentChunk.transform.parent = cubeParent.transform;
            currentChunk = voxels [i];

            voxels [i].SetActive (false); // Hide the current cube*/

            // Establish our arrays for mesh combination
            //MeshFilter[] meshes = currentChunk.GetComponentsInChildren<MeshFilter>(true); // Get all meshFilters in terrain. True to also find deactivated children

            /*int index = -1; // Our voxel count

            // Loop through all children of cube parent
            for (int j = meshes.Length - 1; j >= 0; --j)
            {
                index++;

                MeshFilter meshFilter = meshes [j]; // Link each child meshFilter to a new meshfilter variable
                MeshRenderer meshRenderer = voxels [i].GetComponent <MeshRenderer>();
                string materialName = meshRenderer.material.name;

                CombineInstance combine = new CombineInstance(); // Create a new Combine Instance
                
                // Find which list the cube belongs in
                if (materialName == "Deep Water (Instance)")
                {
                    GameObject listDWHolder = new GameObject();
                    listDWHolder.transform.parent = cubeParent.transform;
                    voxels [index].transform.parent = listDWHolder.transform;

                    voxels [index].GetComponent <MeshRenderer>().material = voxelMat;
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    deepWaterList.Add (combine); // Finally, add our mesh to the matching list
                }
                else if (materialName == "Water (Instance)")
                {
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    shallowWaterList.Add (combine); // Finally, add our mesh to the matching list
                }
                else if (materialName == "Sand (Instance)")
                {
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    sandList.Add (combine); // Finally, add our mesh to the matching list
                }
                else if (materialName == "Low Land (Instance)")
                {
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    lowGrassList.Add (combine); // Finally, add our mesh to the matching list
                }
                else if (materialName == "High Land (Instance)")
                {
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    highGrassList.Add (combine); // Finally, add our mesh to the matching list
                }
                else if (materialName == "Stone (Instance)")
                {
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    stoneList.Add (combine); // Finally, add our mesh to the matching list
                }
                else if (materialName == "Mountain (Instance)")
                {
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    mountainList.Add (combine); // Finally, add our mesh to the matching list
                }
                else if (materialName == "Mountain Tip (Instance)")
                {
                    combine.mesh = meshFilter.mesh; // Match up our mesh with the specific mesh for combination
                    combine.transform = meshFilter.transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves

                    mountainTipList.Add (combine); // Finally, add our mesh to the matching list
                }*/

                combined [i].mesh = meshes [i].sharedMesh; // Combine all the meshes into one shared mesh
                combined [i].transform = meshes [i].transform.localToWorldMatrix; // Keep combined mesh at general coordinate so nothing moves
                meshes [i].gameObject.SetActive (false); // Disable our individual mesh filter now that we have an array containing all combined meshes
            //}
        }
        /*
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

        // Create array that will contain the combined mesh
        CombineInstance[] totalMesh = new CombineInstance [8];

        // Add all submeshes to the combined mesh in the same order. Transform localToWorldMatrix to stop any transform changes in the combination process
        totalMesh[0].mesh = combinedDeepWaterMesh;
        totalMesh[0].transform = cubeParent.transform.localToWorldMatrix;
        totalMesh[1].mesh = combinedShallowWaterMesh;
        totalMesh[1].transform = cubeParent.transform.localToWorldMatrix;
        totalMesh[2].mesh = combinedSandMesh;
        totalMesh[2].transform = cubeParent.transform.localToWorldMatrix;
        totalMesh[3].mesh = combinedLowGrassMesh;
        totalMesh[3].transform = cubeParent.transform.localToWorldMatrix;
        totalMesh[4].mesh = combinedHighGrassMesh;
        totalMesh[4].transform = cubeParent.transform.localToWorldMatrix;
        totalMesh[5].mesh = combinedStoneMesh;
        totalMesh[5].transform = cubeParent.transform.localToWorldMatrix;
        totalMesh[6].mesh = combinedMountainMesh;
        totalMesh[6].transform = cubeParent.transform.localToWorldMatrix;
        totalMesh[7].mesh = combinedMountainTipMesh;
        totalMesh[7].transform = cubeParent.transform.localToWorldMatrix;

        // Create the final combined mesh
        Mesh combinedAllMesh = new Mesh();

        // Finally combine all the meshes. False so that regions are separate
        combinedAllMesh.CombineMeshes (totalMesh, false);
        cubeParent.GetComponent <MeshFilter>().mesh = combinedAllMesh; // Finally set our combined object to render
        */
        
        // If the parent gameObject has no mesh filter then we should give it one
        if (cubeParent.gameObject.GetComponent <MeshFilter>() == null)
        {
            cubeParent.transform.gameObject.AddComponent <MeshFilter>(); // Add a new meshFilter to our parent object
            MeshFilter parentFilter = cubeParent.GetComponent <MeshFilter>();

            // Create and calculate our new, combined mesh
            parentFilter.sharedMesh = new Mesh(); // Set a blank custom mesh to our meshFilter
            parentFilter.sharedMesh.CombineMeshes (combined, true); // Add our array of combined meshes onto this blank mesh and merge the sub-meshes
            parentFilter.sharedMesh.RecalculateBounds(); // Calculate the bounds of the combined mesh
            parentFilter.sharedMesh.RecalculateNormals(); // Calculate the normals of the combined mesh
        }

        // Add a MeshRenderer to our parent object if there isn't one
        if (cubeParent.GetComponent <MeshRenderer>() == null)
        {
            cubeParent.AddComponent <MeshRenderer>();
            MeshRenderer parentRenderer = cubeParent.GetComponent <MeshRenderer>();

            parentRenderer.material = new Material (Shader.Find ("Standard")); // Add a base material
        }
    }
}
