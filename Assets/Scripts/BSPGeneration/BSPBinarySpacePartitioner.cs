using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class BSPBinarySpacePartitioner 
{
    BSPRoomNode _rootNode;
    public BSPRoomNode RootNode=> _rootNode;

    public BSPBinarySpacePartitioner(int dungeonWidth, int dungeonHeight)
    {
        _rootNode= new BSPRoomNode(Vector2Int.zero, new Vector2Int(dungeonWidth, dungeonHeight),null,0);
    }

    public List<BSPRoomNode> PrepareNodeCollection(int maxIterations, int minRoomWidth, int minRoomHeight)
    {
        Queue<BSPRoomNode> graph= new Queue<BSPRoomNode>();
        List<BSPRoomNode> resultList= new List<BSPRoomNode>();

        graph.Enqueue(_rootNode);
        resultList.Add(_rootNode);

        int iterations = 0;

        while (iterations < maxIterations && graph.Count>0)
        {
            iterations++;

            var currentNode=graph.Dequeue();

            if(currentNode.Width >= minRoomWidth*2 || currentNode.Leght>=minRoomHeight*2)
            {
                //siamo dentro lo spazio che ci serve da dividere
                SplitTheSpace(currentNode, resultList, minRoomWidth, minRoomHeight,graph);
            }
        }

        return resultList;
    }

    private void SplitTheSpace(BSPRoomNode currentNode, List<BSPRoomNode> resultList, int minRoomWidth, int minRoomHeight, Queue<BSPRoomNode> graph)
    {
        BSPLine dividingLine=GetDividingLineSpace(currentNode.BottomLeftAreaCorner,currentNode.TopRightAreaCorner,minRoomWidth,minRoomHeight);

        BSPRoomNode node1=null, node2=null;

        switch (dividingLine.Orientation)
        {
            case BSPOrientation.Horizontal:
                node1 = new BSPRoomNode(
                    currentNode.BottomLeftAreaCorner,
                    new Vector2Int(currentNode.TopRightAreaCorner.x, dividingLine.Coordinaters.y),
                    currentNode,
                    currentNode.TreeLayerIndex + 1);

                node2= new BSPRoomNode(
                    new Vector2Int(currentNode.BottomLeftAreaCorner.x,dividingLine.Coordinaters.y),
                    currentNode.TopRightAreaCorner,
                    currentNode,
                    currentNode.TreeLayerIndex + 1);

                break;
            case BSPOrientation.Vertical:
                node1=new BSPRoomNode(
                    currentNode.BottomLeftAreaCorner,
                    new Vector2Int(dividingLine.Coordinaters.x,currentNode.TopRightAreaCorner.y),
                    currentNode,
                    currentNode.TreeLayerIndex + 1);

                node2=new BSPRoomNode(
                    new Vector2Int(dividingLine.Coordinaters.x,currentNode.BottomLeftAreaCorner.y),
                    currentNode.TopRightAreaCorner,
                    currentNode,
                    currentNode.TreeLayerIndex + 1);
                break;
        }

        AddNodesToCollections(resultList, graph, node1);
        AddNodesToCollections(resultList, graph, node2);



    }

    private void AddNodesToCollections(List<BSPRoomNode> resultList,Queue<BSPRoomNode> graph, BSPRoomNode node)
    {
        resultList.Add(node);
        graph.Enqueue(node);
    }

    private BSPLine GetDividingLineSpace(Vector2Int bottonLeftAreaCorner, Vector2Int topRightAreaCorner, int minRoomWidth, int minRoomHeight)
    {
        BSPOrientation orientation;
        bool heightStatus = (topRightAreaCorner.y - bottonLeftAreaCorner.y) >= minRoomHeight * 2;
        bool widthStatus = (topRightAreaCorner.x - bottonLeftAreaCorner.x) >= minRoomWidth * 2;

        if(heightStatus && widthStatus)
        {
            orientation = (BSPOrientation)UnityEngine.Random.Range(0, 2);
        }
        else if (heightStatus)
        {
            orientation = BSPOrientation.Horizontal;
        }
        else
        {
            orientation = BSPOrientation.Vertical;
        }

        return new BSPLine(orientation, GetCordinateForOrientation(orientation, bottonLeftAreaCorner, topRightAreaCorner, minRoomWidth, minRoomHeight));
    }

    private Vector2Int GetCordinateForOrientation(BSPOrientation orientation, Vector2Int bottonLeftAreaCorner, Vector2Int topRightAreaCorner, int minRoomWidth, int minRoomHeight)
    {
        Vector2Int coordinates = Vector2Int.zero;

        switch (orientation)
        {
            case BSPOrientation.Horizontal:
                coordinates=new Vector2Int(0,UnityEngine.Random.Range(bottonLeftAreaCorner.y + minRoomHeight, topRightAreaCorner.y - minRoomHeight));
                break;
            case BSPOrientation.Vertical:
                coordinates=new Vector2Int(UnityEngine.Random.Range(bottonLeftAreaCorner.x + minRoomWidth, topRightAreaCorner.x - minRoomWidth),0);
                break;
        }

        return coordinates;
    }
}
