using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSPDungeonGenerator
{
    public int DungeonWidth;
    public int DungeonHeight;

    private List<BSPRoomNode> _allRoomNodeList;

    public BSPDungeonGenerator(int dungeonWidth, int dungeonHeight)
    {
        DungeonWidth = dungeonWidth;
        DungeonHeight = dungeonHeight;
        _allRoomNodeList = new List<BSPRoomNode>();
    }

    public List<BSPNode> CalculateDungeon(int maxIterations, int minRoomWidth, int minRoomHeight, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset,int corridorWidth)
    {
        BSPBinarySpacePartitioner bsp= new BSPBinarySpacePartitioner(DungeonWidth,DungeonHeight);
        _allRoomNodeList=bsp.PrepareNodeCollection(maxIterations,minRoomWidth,minRoomHeight);

        List<BSPNode> roomSpaces = BSPStructureHelper.GetLowestLeavesFromGraph(bsp.RootNode);
        List<BSPRoomNode> roomList= BSPRoomGenerator.GenerateRoomInGivenSpace(roomSpaces,roomBottomCornerModifier, roomTopCornerModifier, roomOffset);

        CorridorsGenerator corridorGenerator=new CorridorsGenerator();
        var corridorList = corridorGenerator.CreateCorridor(_allRoomNodeList, corridorWidth);
       

        return new List<BSPNode>(roomList).Concat(corridorList).ToList();
    }
}
