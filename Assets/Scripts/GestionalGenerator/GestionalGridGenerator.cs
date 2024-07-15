using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GestionalGridGenerator : MonoBehaviour,ISalvable
{
    //[SerializeField] WorldSection worldMap;

    public int GridWidth;
    public int GridHeight;
    public int TileCount;
    public Material TileMaterial;
    public LayerMask tileLayer;

    public int innerTileCount;
    public Material innerTileMaterial;
    public LayerMask innerTileLayer;

    //[SerializeField] List<WorldSection> WorldSectionList = new();

    [SerializeField] List<GameObject> EnviromentDecoration;
    [Range(0f, 1f)]
    [SerializeField] float DecorationDensity = 0.1f;
    [SerializeField] float noiseScale = 1f;

    public Dictionary<GestionalTile, List<GestionalTile>> WorldSectionTileMap = new();

    private void Start()
    {
        //if (WorldSectionList.Count == 0)
        //    return;

        List<GestionalTile> gestionalTiles = GenerateGrid(gameObject, GridWidth, GridHeight, TileCount, TileMaterial,GestionalTileType.Structure,tileLayer);

        foreach (GestionalTile tile in gestionalTiles)
        {
            
            WorldSectionTileMap.Add(tile, GenerateGrid(tile.gameObject, (int)tile.SquareDimension, (int)tile.SquareDimension, innerTileCount, innerTileMaterial, GestionalTileType.Other,innerTileLayer));
        }

        foreach(GestionalTile tile in gestionalTiles)
        {
            tile.CheckForSubTiles();
        }

        foreach (GestionalTile tile in gestionalTiles)
        {
            foreach(GestionalTile subtile in tile.subTileList)
            {
                if(subtile != null && !subtile.Occupied)
                {
                    GenerateEnviromentDecorations(subtile);
                }
            }
        }

    }

    public void GenerateEnviromentDecorations(GestionalTile subtile)
    {
        Vector3 subTilePosition = subtile.transform.position;

        float objectNoise = Mathf.PerlinNoise(subTilePosition.x * noiseScale, subTilePosition.z * noiseScale);

        if (objectNoise < DecorationDensity)
        {
            GameObject decorationGameObject = Instantiate(EnviromentDecoration[UnityEngine.Random.Range(0, EnviromentDecoration.Count)], subtile.transform);

            subtile.SetObjectOnTile(decorationGameObject);
        }
    }

    public static List<GestionalTile> GenerateGrid(GameObject parent, int gridWidth, int gridHeight, int tileSquareCount, Material tileMaterial,GestionalTileType type,LayerMask layer)
    {
        List<GestionalTile> resultList = new List<GestionalTile>();
        float tileSizeX = (float)gridWidth / tileSquareCount;
        float tileSizeZ = (float)gridHeight / tileSquareCount;

        for (int x = 0; x < tileSquareCount; x++)
        {
            for (int z = 0; z < tileSquareCount; z++)
            {
                Mesh newMesh = GenerateNewMesh(tileSizeX, tileSizeZ);

                GameObject newTile = new GameObject($"Tile {x}, {z}");
                newTile.transform.parent = parent.transform;
                newTile.transform.localPosition = new Vector3(x * tileSizeX, 0, z * tileSizeZ);
                
                newTile.layer = GetLayerFromMask(layer);

                MeshFilter meshFilter = newTile.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = newTile.AddComponent<MeshRenderer>();
                MeshCollider meshCollider= newTile.AddComponent<MeshCollider>();

                meshFilter.mesh = newMesh;
                meshCollider.sharedMesh = newMesh;

                meshRenderer.material = tileMaterial;

                GestionalTile gestionalTile = newTile.AddComponent<GestionalTile>();
                gestionalTile.SquareDimension = tileSizeX;
                gestionalTile.TileType = type;
                
                

                resultList.Add(gestionalTile);
            }
        }

        //check nearby tile

        foreach(GestionalTile tile in resultList)
        {
             tile.CheckForNeighbour(layer);
        }
       


        return resultList;
    }

    public static Mesh GenerateNewMesh(float xPosition, float zPosition)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(xPosition,0,0),
            new Vector3(0,0,zPosition),
            new Vector3(xPosition,0,zPosition),
        };

        int[] triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        //genero le uv per il material
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        mesh.uv = uvs;

        return mesh;
    }

    public static int GetLayerFromMask(LayerMask layerMask)
    {
        int layer = 0;
        int mask = layerMask.value;
        while (mask > 1)
        {
            mask >>= 1;
            layer++;
        }
        return layer;
    }

    public void OnSave()
    {
        SaveManager saveManager= FindObjectOfType<SaveManager>();

        saveManager.gridSave.GridWidth = GridWidth;
        saveManager.gridSave.GridHeight = GridHeight;
        saveManager.gridSave.TileCount = TileCount;

        saveManager.gridSave.tiles.Clear();

        foreach(GestionalTile tile in WorldSectionTileMap.Keys)
        {
            SaveManager.TileSave tileSave=new SaveManager.TileSave();
            tileSave.Occupied=tile.Occupied;

            if (tile.Occupied)
            {
                tileSave.ObjectName = tile.objectOnTile.name.Replace("(Clone)", ""); ;
                tileSave.objectRotationX=tile.objectOnTile.transform.eulerAngles.x;
                tileSave.objectRotationY = tile.objectOnTile.transform.eulerAngles.y;
                tileSave.objectRotationZ = tile.objectOnTile.transform.eulerAngles.z;
            }

            tileSave.innerTileSaves = new();

            foreach (GestionalTile innerTile in tile.subTileList)
            {
                SaveManager.innerTileSave innerTileSave=new SaveManager.innerTileSave();
                innerTileSave.Occupied = innerTile.Occupied;

                if (innerTile.Occupied)

                {
                    innerTileSave.ObjectName = innerTile.objectOnTile.name.Replace("(Clone)","");
                    innerTileSave.objectRotationX = innerTile.objectOnTile.transform.eulerAngles.x;
                    innerTileSave.objectRotationY = innerTile.objectOnTile.transform.eulerAngles.y;
                    innerTileSave.objectRotationZ = innerTile.objectOnTile.transform.eulerAngles.z;
                }

                tileSave.innerTileSaves.Add(innerTileSave);
            }

            saveManager.gridSave.tiles.Add(tileSave);
        }



        saveManager.SaveToJson();
    }

    public void OnLoad()
    {
        SaveManager saveManager = FindObjectOfType<SaveManager>();

        saveManager.LoadFromJson();

        GridWidth = saveManager.gridSave.GridWidth;
        GridHeight = saveManager.gridSave.GridHeight;
        TileCount = saveManager.gridSave.TileCount;

        //rifaccio la griglia con i nuovi valori caricati
        //prima disabilito e poi distruggo se no dà problemi
        foreach (GestionalTile tile in WorldSectionTileMap.Keys)
        {
            tile.gameObject.SetActive(false);

        }

        foreach (GestionalTile tile in WorldSectionTileMap.Keys)
        {
            Destroy(tile.gameObject);

        }

        WorldSectionTileMap.Clear();

        List<GestionalTile> gestionalTiles = GenerateGrid(gameObject, GridWidth, GridHeight, TileCount, TileMaterial, GestionalTileType.Structure, tileLayer);

        foreach (GestionalTile tile in gestionalTiles)
        {

            WorldSectionTileMap.Add(tile, GenerateGrid(tile.gameObject, (int)tile.SquareDimension, (int)tile.SquareDimension, innerTileCount, innerTileMaterial, GestionalTileType.Other, innerTileLayer));
        }

        foreach (GestionalTile tile in gestionalTiles)
        {
            tile.CheckForSubTiles();
        }

        //setto il primo layer della griglia (per le strade)

        for (int i = 0; i < gestionalTiles.Count; i++)
        {
            if (saveManager.gridSave.tiles[i].Occupied)
            {
                GameObject gameObjectFromResources = Resources.Load(saveManager.gridSave.tiles[i].ObjectName) as GameObject;

                if (gameObjectFromResources.GetComponent<RoadHandler>() == null)
                {
                    continue;
                }



                GameObject gameObject = Instantiate(gameObjectFromResources, gestionalTiles[i].transform);


                gestionalTiles[i].SetObjectOnTile(gameObject);
                gameObject.transform.eulerAngles = new Vector3(saveManager.gridSave.tiles[i].objectRotationX, saveManager.gridSave.tiles[i].objectRotationY, saveManager.gridSave.tiles[i].objectRotationZ);
            }


        }

        //setto le altre strutture al primo layer
        for (int i = 0; i < gestionalTiles.Count; i++)
        {
            if (saveManager.gridSave.tiles[i].Occupied)
            {
                GameObject gameObjectFromResources = Resources.Load(saveManager.gridSave.tiles[i].ObjectName) as GameObject;

                if (gameObjectFromResources.GetComponent<RoadHandler>() != null)
                {
                    continue;
                }



                GameObject gameObject = Instantiate(gameObjectFromResources, gestionalTiles[i].transform);


                gestionalTiles[i].SetObjectOnTile(gameObject);
                gameObject.transform.eulerAngles = new Vector3(saveManager.gridSave.tiles[i].objectRotationX, saveManager.gridSave.tiles[i].objectRotationY, saveManager.gridSave.tiles[i].objectRotationZ);
            }
        }

         StartCoroutine(DelayCleanAndCreationOfDecorations(saveManager, gestionalTiles));

    }

    private IEnumerator DelayCleanAndCreationOfDecorations(SaveManager saveManager, List<GestionalTile> gestionalTiles)
    {
        yield return new WaitForEndOfFrame();
        //pulisco le decorazioni create randomicamente
        foreach (GestionalTile Tile in gestionalTiles)
        {
            foreach (GestionalTile innerTile in Tile.subTileList)
            {
                Debug.Log("bananananana");
                if (innerTile.objectOnTile != null)
                {

                    Destroy(innerTile.objectOnTile);
                    innerTile.Occupied = false;
                    innerTile.objectOnTile = null;
                }
            }
        }





        //creo le decorazioni salvate

        for (int i = 0; i < gestionalTiles.Count; i++)
        {
            for (int j = 0; j < gestionalTiles[i].subTileList.Count; j++)
            {
                if (saveManager.gridSave.tiles[i].innerTileSaves[j].Occupied)
                {
                    GameObject gameObjectFromResources;

                    if (gestionalTiles[i].Occupied)
                    {
                        gameObjectFromResources = Resources.Load("Decoration/Road/" + saveManager.gridSave.tiles[i].innerTileSaves[j].ObjectName) as GameObject;
                    }
                    else
                    {
                        gameObjectFromResources = Resources.Load("Decoration/Nature/" + saveManager.gridSave.tiles[i].innerTileSaves[j].ObjectName) as GameObject;
                    }

                    GameObject gameObject = Instantiate(gameObjectFromResources, gestionalTiles[i].subTileList[j].transform);

                    gestionalTiles[i].subTileList[j].SetObjectOnTile(gameObject);
                    gameObject.transform.Rotate(new Vector3(saveManager.gridSave.tiles[i].innerTileSaves[j].objectRotationX, saveManager.gridSave.tiles[i].innerTileSaves[j].objectRotationY, saveManager.gridSave.tiles[i].innerTileSaves[j].objectRotationZ));
                }
            }
        }
    }
}


[Serializable]
public class WorldSection
{
    public int TileCount;
    public Material TileMaterial;

    public Dictionary<GestionalTile, List<GestionalTile>> WorldSectionTileMap { get; set; }

    public WorldSection()
    {
        WorldSectionTileMap = new Dictionary<GestionalTile, List<GestionalTile>>();
    }

    public void AddNewTile(GestionalTile tile, List<GestionalTile> gestionalTiles = null)
    {
        if (WorldSectionTileMap.ContainsKey(tile))
            return;

        WorldSectionTileMap.Add(tile, gestionalTiles);
    }

    public void RemoveTile(GestionalTile tile)
    {
        if (!WorldSectionTileMap.ContainsKey(tile))
            return;

        WorldSectionTileMap.Remove(tile);
    }




}