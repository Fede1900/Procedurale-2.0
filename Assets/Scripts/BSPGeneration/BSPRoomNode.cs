using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPRoomNode : BSPNode
{
    public int Width { get=> TopRightAreaCorner.x - BottomLeftAreaCorner.x; }
    public int Leght { get=> TopRightAreaCorner.y - BottomLeftAreaCorner.y; }
    public BSPRoomNode(Vector2Int bottonLeftAreaCorner, Vector2Int topRightAreaCorner, BSPNode parent, int layerIndex) : base(parent)
    {
        BottomLeftAreaCorner = bottonLeftAreaCorner;
        TopRightAreaCorner = topRightAreaCorner;
        BottomRightAreaCorner = new Vector2Int(TopRightAreaCorner.x,BottomLeftAreaCorner.y);
        TopLeftAreaCorner= new Vector2Int(BottomLeftAreaCorner.x,TopRightAreaCorner.y);
        TreeLayerIndex = layerIndex;
    }
}
