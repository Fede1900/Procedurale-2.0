using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum GestionalTileType
{
    Structure,
    Other
}

public class GestionalTile : MonoBehaviour,IGestionalSelectable
{    

    public float SquareDimension;

    public bool Occupied;

    public GestionalTileType TileType;

    public GameObject objectOnTile;

    public List<GestionalTile> tileNearbyList;

    public GestionalGridGenerator gestionalGridGenerator;

    public List<GestionalTile> subTileList;

    public GestionalTile[,] subtiles;


    //nearTile

    public GestionalTile northTile;
    public GestionalTile southTile;
    public GestionalTile eastTile;
    public GestionalTile westTile;

    public GestionalTile north_eastTile;
    public GestionalTile south_eastTile;
    public GestionalTile north_westTile;
    public GestionalTile south_westTile;

    private void Awake()
    {
        
        gestionalGridGenerator=GetComponentInParent<GestionalGridGenerator>();
        subtiles = new GestionalTile[gestionalGridGenerator.innerTileCount, gestionalGridGenerator.innerTileCount];
    }

    public void OnDeselect(Action action)
    {
        action.Invoke();
    }

    public void OnSelect(Action action)
    {
        action.Invoke();
    }

    public void CheckForNeighbour(LayerMask layer)
    {
        tileNearbyList= new List<GestionalTile>();

        Vector3 centerTileTransform= transform.position+new Vector3(SquareDimension/2,0, SquareDimension / 2);
        RaycastHit hit;

        //check noth
        if(Physics.Raycast((centerTileTransform+Vector3.up)+(Vector3.forward*SquareDimension),Vector3.down,out hit,1000,layer)) 
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                northTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(northTile);
            }
        }

        //check south
        if (Physics.Raycast((centerTileTransform + Vector3.up) + (Vector3.back * SquareDimension), Vector3.down, out hit, 1000,layer))
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                southTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(southTile);
            }
        }

        //check west
        if (Physics.Raycast((centerTileTransform + Vector3.up) + (Vector3.left * SquareDimension), Vector3.down, out hit, 1000, layer))
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                westTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(westTile);
            }
        }

        //check east
        if (Physics.Raycast((centerTileTransform + Vector3.up) + (Vector3.right * SquareDimension), Vector3.down, out hit, 1000,layer))
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                eastTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(eastTile);
            }
        }

        //check northEast
        if (Physics.Raycast((centerTileTransform + Vector3.up) + ((Vector3.right+Vector3.forward) * SquareDimension), Vector3.down, out hit, 1000, layer))
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                north_eastTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(north_eastTile);
            }
        }

        //check northWest
        if (Physics.Raycast((centerTileTransform + Vector3.up) + ((Vector3.left + Vector3.forward) * SquareDimension), Vector3.down, out hit, 1000, layer))
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                north_westTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(north_westTile);
            }
        }

        //check southEast
        if (Physics.Raycast((centerTileTransform + Vector3.up) + ((Vector3.right + Vector3.back) * SquareDimension), Vector3.down, out hit, 1000, layer))
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                south_eastTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(south_eastTile);
            }
        }

        //check southWest
        if (Physics.Raycast((centerTileTransform + Vector3.up) + ((Vector3.left + Vector3.back) * SquareDimension), Vector3.down, out hit, 1000, layer))
        {
            if (hit.transform.gameObject.GetComponent<GestionalTile>() != null)
            {
                south_westTile = hit.transform.gameObject.GetComponent<GestionalTile>();
                tileNearbyList.Add(south_westTile);
            }
        }
    }

    public void CheckForSubTiles()
    {
        subTileList= new List<GestionalTile>();
        
        if (gestionalGridGenerator.WorldSectionTileMap.TryGetValue(this, out subTileList))
        {
            int x = 0;
            int y = 0;

            foreach (GestionalTile tile in subTileList)
            {

                subtiles[x, y] = tile;

                if (x < subtiles.GetLength(0))
                {
                    y++;
                    if (y>= subtiles.GetLength(1))
                    {
                        x++;
                        y = 0;
                    }
                    
                }
            }

        }
    }

    public void SetObjectOnTile(GameObject gameObject)
    {
        Vector3 centerOfTile = new Vector3(SquareDimension / 2, 0 ,SquareDimension / 2);
        gameObject.transform.localPosition = centerOfTile;
        Occupied=true;
        objectOnTile = gameObject;
    }

    

    

    
}
