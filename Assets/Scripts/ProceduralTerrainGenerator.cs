using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] int width=10;  
    [SerializeField] int length=10;
    [SerializeField] float maxHeight;


    [Header("SecondLayer")]
    [SerializeField] List<PerlinSettings> perlinSettings;
    [Header("EnviromentObjects")]
    [SerializeField] List<GameObject> treesPrefabList;
    [SerializeField] List<GameObject> rockPrefabList;
    [SerializeField] List<GameObject> bushesPrefabList;

    [SerializeField] float treesDensity=0.1f;
    [SerializeField] float rocksDensity=0.2f;
    [SerializeField] float bushesDensity=0.3f;

    [SerializeField] float treesNoiseScale = 0.1f;
    [SerializeField] float rocksNoiseScale = 0.1f;
    [SerializeField] float bushesNoiseScale = 0.1f;


    private void Start()
    {
        populateTerrain(terrain.terrainData);
    }

   

    private void OnValidate()
    {
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    private TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width;
        terrainData.size = new Vector3(width, maxHeight, length);

        float[,] heights = new float[width, length];

        foreach(var settings in perlinSettings)
        {
            HeightsPerlin(ref heights, width, length, settings.MinHeight, settings.MaxHeight, settings.Scale,settings.Seed, settings.MaxRandomness, settings.UseLerp,settings.substract);
        }     

        terrainData.SetHeights(0, 0, heights);


        return terrainData;
    }

    private void HeightsPerlin(ref float[,] heights,int width,int length,float minHeigth,float maxHeight,float scale,int seed, int maxRandomness,bool useLerp,bool substract)
    {
        

        System.Random random = new System.Random(seed);

        float offsetX = random.Next(0, maxRandomness);
        float offsetY = random.Next(0, maxRandomness);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                float xCoordinate = (float)x / width * scale + offsetX;
                float yCoordinate = (float)z / length * scale + offsetY;
                float perlinHeightsValue = Mathf.PerlinNoise(xCoordinate, yCoordinate);
                if (useLerp)
                {
                    heights[x, z] += substract? -Mathf.Lerp(minHeigth, maxHeight, perlinHeightsValue) : Mathf.Lerp(minHeigth, maxHeight, perlinHeightsValue);
                }
                else
                {
                    heights[x, z] += substract? - Mathf.Clamp(perlinHeightsValue,minHeigth,maxHeight) : Mathf.Clamp(perlinHeightsValue, minHeigth, maxHeight);
                }
                
            }
        }
    }

    private void populateTerrain(TerrainData terrainData)
    {
        bool[,] objectMap = new bool[width, length];

        for(int x = 0;x < width; x++)
        {
            for(int z = 0;z < length; z++)
            {
                float currentHeight=terrainData.GetHeight(x, z);

                Vector3 spawnPosition= new Vector3(x,currentHeight,z);

                float treeNoise= Mathf.PerlinNoise(x * treesNoiseScale,z * treesNoiseScale);
                float rockNoise = Mathf.PerlinNoise(x * rocksNoiseScale, z * rocksNoiseScale);
                float bushNoise = Mathf.PerlinNoise(x * bushesNoiseScale, z * bushesNoiseScale);

                if(treeNoise< treesDensity)
                {
                      SpawnObjectOnMap(objectMap, x, z, spawnPosition,treesPrefabList);                  
                }
                else if (rockNoise < rocksDensity)
                {
                    SpawnObjectOnMap(objectMap, x, z, spawnPosition, rockPrefabList);
                }
                else if(bushNoise < bushesDensity)
                {
                    SpawnObjectOnMap(objectMap, x, z, spawnPosition, bushesPrefabList);
                }
            }
        }
    }

    private void SpawnObjectOnMap(bool[,] objectMap, int x, int z, Vector3 spawnPosition,List<GameObject> prefabList)
    {
        GameObject newTree = Instantiate(prefabList[UnityEngine.Random.Range(0, prefabList.Count)]);
        newTree.transform.position = spawnPosition;

        objectMap[x, z] = true;
    }
     
    [Serializable]
    public class PerlinSettings
    {
        public float MinHeight;
        public float MaxHeight;
        public int Seed;
        public int MaxRandomness;
        public bool UseLerp;
        public float Scale;
        public bool substract;

    }


}
