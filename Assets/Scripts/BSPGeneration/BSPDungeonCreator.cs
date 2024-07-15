using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonCreator : MonoBehaviour
{
    public int DungeonWidth;
    public int DungeonHeight;
    public int MinRoomWidth;
    public int MinRoomHeight;
    public int MaxIterations = 10; //non esagerare con le dimensioni   

    [Range(0f, 0.3f)]
    public float RoomBottomCornerModifier;
    [Range(0.7f, 1f)]
    public float RoomTopCornerModifier;
    [Range(0f, 2f)]
    public int RoomOffset;

    public Material FloorMaterial;

    public int CorridorWidth=5;

    [Header("Wall Settings")]
    [SerializeField] GameObject wallVertical, wallHorizontal;

    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;

    void Start()
    {
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        var dungeonGenerator = new BSPDungeonGenerator(DungeonWidth, DungeonHeight);

        //preparare il  dungeon=lista di stanze
        List<BSPNode> roomList = dungeonGenerator.CalculateDungeon(MaxIterations,MinRoomWidth,MinRoomHeight,RoomBottomCornerModifier,RoomTopCornerModifier,RoomOffset,CorridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.SetParent(transform);

        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        //creare la mesh delle stanze
        foreach (BSPNode node in roomList)
        {
            CreateMesh(node.BottomLeftAreaCorner, node.TopRightAreaCorner);
        }

        

        CreateWalls(wallParent);

    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            Instantiate(wallHorizontal, wallPosition, Quaternion.identity, wallParent.transform);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            Instantiate(wallVertical, wallPosition, Quaternion.identity, wallParent.transform);
        }
    }

    private void CreateMesh(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner)
    {
        Vector3 bottomLeftVertex=new Vector3(bottomLeftAreaCorner.x,0,bottomLeftAreaCorner.y);
        Vector3 bottomRightVertex=new Vector3(topRightAreaCorner.x,0, bottomLeftAreaCorner.y);
        Vector3 topLeftVertex = new Vector3(bottomLeftAreaCorner.x, 0, topRightAreaCorner.y);
        Vector3 topRightVertex = new Vector3(topRightAreaCorner.x, 0, topRightAreaCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftVertex,
            topRightVertex,
            bottomLeftVertex,
            bottomRightVertex
        };

        Vector2[] uvs = new Vector2[vertices.Length];

        for(int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,1,2,2,1,3
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject($"Room_{bottomLeftAreaCorner}",typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = FloorMaterial;

        // eseguiamo il codice sottostanze solo per pigrizia

        //orizzontale
        for(int row=(int)bottomLeftVertex.x;row<(int)bottomRightVertex.x;row++)
        {
            var wallPosition=new Vector3(row,0,bottomLeftVertex.z);
            AddWallPositionList(wallPosition, possibleDoorHorizontalPosition, possibleWallHorizontalPosition);
        }
        for (int row = (int)topLeftVertex.x; row < (int)topRightVertex.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightVertex.z);
            AddWallPositionList(wallPosition, possibleDoorHorizontalPosition, possibleWallHorizontalPosition);
        }

        //verticale
        for (int col = (int)bottomLeftVertex.z; col < (int)topLeftVertex.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftVertex.x, 0, col);
            AddWallPositionList(wallPosition, possibleDoorVerticalPosition, possibleWallVerticalPosition);
        }
        for (int col = (int)bottomRightVertex.z; col < (int)topRightVertex.z; col++)
        {
            var wallPosition = new Vector3(bottomRightVertex.x, 0, col);
            AddWallPositionList(wallPosition, possibleDoorVerticalPosition, possibleWallVerticalPosition);
        }

    }

    private void AddWallPositionList(Vector3 wallPosition, List<Vector3Int> doorSpaceList, List<Vector3Int> wallPositionList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if(wallPositionList.Contains(point))
        {
            doorSpaceList.Add(point);
            wallPositionList.Remove(point);
        }
        else
        {
            wallPositionList.Add(point);
        }
    }
} 
