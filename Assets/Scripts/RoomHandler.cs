using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class RoomHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshCollider tileableFloor;

    [Header("Assets reference")]
    [SerializeField] WorldTileHandler worldTileHandlerPrefab;
    [SerializeField] List<RoomObjectHandler> roomObjectsPrefabs;

    [Header("Settings")]
    [SerializeField] int minObjectCount;
    [SerializeField] int maxObjectCount;
    [Tooltip("Numero di tentativi per il posizionamento degli oggetti. Dopo questo numero non verrano inseriti ulteriori oggetti")]
    [SerializeField] int placementTries;

    [Range(0f, 1f)]
    [SerializeField] float roomObjectDensity;
    [SerializeField] float roomObjectNoiseScale;

    public WorldTileHandler[,] RoomTiles;

    List<RoomObjectHandler> _spawnedObjects=new List<RoomObjectHandler>();

    private void Awake()
    {
        Destroy(tileableFloor);
    }

    private void Start()
    {
        
    }

    public void Initialize()
    {
        Vector3 floorSize=tileableFloor.bounds.size;
        Vector3 floorMin= tileableFloor.bounds.min;

        Vector3 tileSize = worldTileHandlerPrefab.size;

        int tilesX=Mathf.FloorToInt(floorSize.x/tileSize.x);
        int tilesZ=Mathf.FloorToInt(floorSize.z/tileSize.z);

        RoomTiles=new WorldTileHandler[tilesX,tilesZ];

        for(int x = 0; x < tilesX; x++)
        {
            for(int z=0; z < tilesZ; z++)
            {
                Vector3 tilePosition=floorMin+ new Vector3(tileSize.x*x + tileSize.x/2,0,tileSize.z*z+tileSize.z/2);
                var newTile = Instantiate(worldTileHandlerPrefab, tilePosition, Quaternion.identity);
                newTile.transform.SetParent(transform);
                RoomTiles[x,z] = newTile;
            }
        }

        populateRoom();
    }

    private void populateRoom()
    {
        int counter = 0;
        do
        {
            for (int x = 0; x < RoomTiles.GetLength(0); x++)
            {
                for (int z = 0; z < RoomTiles.GetLength(1); z++)
                {
                    WorldTileHandler currentTile = RoomTiles[x, z];
                    if (currentTile.occupied)
                    {
                        continue;
                    }

                    Vector3 spawnPoint = currentTile.transform.position;

                    float objectNoise = Mathf.PerlinNoise(spawnPoint.x * roomObjectNoiseScale, spawnPoint.z * roomObjectNoiseScale);

                    if (objectNoise < roomObjectDensity && _spawnedObjects.Count<=maxObjectCount)
                    {
                        SpawnObjectOnRoom(currentTile, x, z, spawnPoint, roomObjectsPrefabs);

                        
                    }
                }
            }

            counter++;
        }
        while (counter < placementTries);
        //while (_spawnedObjects.Count < minObjectCount || counter < placementTries || _spawnedObjects.Count >= maxObjectCount);

        
    }

    private void SpawnObjectOnRoom(WorldTileHandler currentTile, int x, int z, Vector3 spawnPosition, List<RoomObjectHandler> prefabList)
    {
        if (!currentTile.occupied)
        {
            RoomObjectHandler newObject = Instantiate(prefabList[UnityEngine.Random.Range(0, prefabList.Count)]);
            newObject.transform.position = spawnPosition;
            currentTile.occupied = true;
            newObject.transform.SetParent(transform);
            _spawnedObjects.Add(newObject);
        }
    }
}
