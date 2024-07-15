using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPRoomGenerator 
{
    public static List<BSPRoomNode> GenerateRoomInGivenSpace(List<BSPNode> roomSpaces, float roomBottomCornerModifier, float roomTopCornerModifier, int offset)
    {
        List<BSPRoomNode> resultList = new List<BSPRoomNode>();

        foreach (BSPNode space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = BSPStructureHelper.GenerateBottomLeftCorner(
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomBottomCornerModifier, offset);

            Vector2Int newTopRightPoint=BSPStructureHelper.GenerateTopRightAreaCorner(
                space.BottomLeftAreaCorner,space.TopRightAreaCorner,roomTopCornerModifier, offset);

            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

            resultList.Add((BSPRoomNode)space);
        }

        return resultList;
    }
}
