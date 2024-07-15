using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadType
{
    normalRoad,
    intersection,
    highway
}

public enum HighwayDirection
{
    Horizontal,
    Vertical,
    None
}

public class RoadHandler : MonoBehaviour
{
    [Header("Normal road neighbour checker")]
    [SerializeField] bool _northOccupied;
    [SerializeField] bool _southOccupied;
    [SerializeField] bool _westOccupied;
    [SerializeField] bool _eastOccupied;

    [Header("highway neighbour checker")]
    [SerializeField] bool _northEastOccupied;
    [SerializeField] bool _northWestOccupied;
    [SerializeField] bool _southEastOccupied;
    [SerializeField] bool _southWestOccupied;

    [Header("stopLight")]
    [SerializeField] GameObject stopLight;

    [Header("decoration")]
    [Range(0f, 1f)]
    [SerializeField] float decorationDensity = 0.1f;
    [SerializeField] float noiseScale = 1f;
    [SerializeField] List<GameObject> normalRoadDecoration;
    [SerializeField] List <GameObject> highWayDecoration;

    public bool NorthOccupied
    {
        get { return _northOccupied; }
        set { _northOccupied = value; 
        northRoad.SetActive(value);
        northWalkway.SetActive(!value);}
    }

    public bool SouthOccupied
    {
        get { return _southOccupied; }
        set
        {
            _southOccupied = value;
            southRoad.SetActive(value);
            southWalkway.SetActive(!value);
        }
    }

    public bool WestOccupied
    {
        get { return _westOccupied; }
        set
        {
            _westOccupied = value;
            westRoad.SetActive(value);
            westWalkway.SetActive(!value);
        }
    }

    public bool EastOccupied
    {
        get { return _eastOccupied; }
        set
        {
            _eastOccupied = value;
            eastRoad.SetActive(value);
            eastWalkway.SetActive(!value);
        }
    }

    public bool NorthEastOccupied
    {
        get { return _northEastOccupied; }
        set
        {
            _northEastOccupied = value;
            northEastRoad.SetActive(value);

            foreach(GameObject g in northEastWalkWay)
            {
                g.SetActive(!value);
            }
        }
    }

    public bool NorthWestOccupied
    {
        get { return _northWestOccupied; }
        set
        {
            _northWestOccupied = value;
            northWestRoad.SetActive(value);

            foreach (GameObject g in northWestWalkWay)
            {
                g.SetActive(!value);
            }
        }
    }

    public bool SouthEastOccupied
    {
        get { return _southEastOccupied; }
        set
        {
            _southEastOccupied = value;
            southEastRoad.SetActive(value);

            foreach (GameObject g in southEastWalkWay)
            {
                g.SetActive(!value);
            }
        }
    }

    public bool SouthWestOccupied
    {
        get { return _southWestOccupied; }
        set
        {
            _southWestOccupied = value;
            southWestRoad.SetActive(value);

            foreach (GameObject g in southWestWalkWay)
            {
                g.SetActive(!value);
            }
        }
    }



    [Header("gameobjectProcedural")]
    [Header("north")]
    [SerializeField] GameObject northRoad;
    [SerializeField] GameObject northWalkway;
    [Header("south")]
    [SerializeField] GameObject southRoad;
    [SerializeField] GameObject southWalkway;
    [Header("west")]
    [SerializeField] GameObject westRoad;
    [SerializeField] GameObject westWalkway;
    [Header("east")]
    [SerializeField] GameObject eastRoad;
    [SerializeField] GameObject eastWalkway;

    [Header("northEast")]
    [SerializeField] GameObject northEastRoad;
    [SerializeField] List<GameObject> northEastWalkWay;
    [Header("northWest")]
    [SerializeField] GameObject northWestRoad;
    [SerializeField] List<GameObject> northWestWalkWay;
    [Header("southEast")]
    [SerializeField] GameObject southEastRoad;
    [SerializeField] List<GameObject> southEastWalkWay;
    [Header("southWest")]
    [SerializeField] GameObject southWestRoad;
    [SerializeField] List<GameObject> southWestWalkWay;



    [Header("Highway")]
    [SerializeField] GameObject horizontalUpLines;
    [SerializeField] GameObject horizontalDownLines;
    [SerializeField] List<GameObject> verticalsLines;

    [SerializeField] GameObject verticalLeftLines;
    [SerializeField] GameObject verticalRightLines;
    [SerializeField] List<GameObject> horizontalLines;

    public RoadType roadType=RoadType.normalRoad;
    public HighwayDirection highwayDirection= HighwayDirection.None;

    public GestionalTile tileOwner;



    private void Awake()
    {
        tileOwner = GetComponentInParent<GestionalTile>();

        
    }
    private void Start()
    {
        AdjustNearbyRoad();
    }

    public void AdjustNearbyRoad()
    {
        //setto prima quella attuale

        AdjustRoad();

        foreach(GestionalTile tile in tileOwner.tileNearbyList)
        {
            if(tile.objectOnTile!=null)
            {
                ChangeNearbyTile(tile);
            }
        }
    }

    private void AdjustRoad()
    {
        RoadType adjustType = RoadType.normalRoad;      
        highwayDirection = HighwayDirection.None;

        //pulisco le decorazioni al ricalcolo della strada
        foreach(GestionalTile tile in tileOwner.subTileList)
        {
            if (tile.objectOnTile != null)
            {                
                Destroy(tile.objectOnTile);
                tile.Occupied = false;
                tile.objectOnTile = null;
            }
        }
        

        NorthOccupied = false;
        SouthOccupied = false;
        WestOccupied = false;
        EastOccupied = false;

        NorthEastOccupied = false;
        NorthWestOccupied = false;
        SouthEastOccupied = false;
        SouthWestOccupied = false;

        #region parte usata per il tentativo raggruppamento autostrada
        horizontalUpLines.SetActive(false);
         horizontalDownLines.SetActive(false);
        
         verticalLeftLines.SetActive(false);
         verticalRightLines.SetActive(false);

        foreach(GameObject g in verticalsLines)
        {
            g.SetActive(true);
        }
        
        foreach(GameObject g in horizontalLines)
        {
            g.SetActive(true);
        }
        #endregion

        //parte di controllo n,s,w,e

        int numberVerticalNearby = 0;
        int numberDiagonalNearby = 0;

        if(tileOwner.northTile!=null)
        {
            if (tileOwner.northTile.objectOnTile!=null && tileOwner.northTile.objectOnTile.GetComponent<RoadHandler>()!=null)
            {
                NorthOccupied = true;
                numberVerticalNearby++;
            }
           
        }
        

        if (tileOwner.southTile != null)
        {
            if (tileOwner.southTile.objectOnTile != null && tileOwner.southTile.objectOnTile.GetComponent<RoadHandler>() != null)
            {
                SouthOccupied = true;
                numberVerticalNearby++;
            }
            
        }
        

        if (tileOwner.westTile != null)
        {
            if (tileOwner.westTile.objectOnTile != null && tileOwner.westTile.objectOnTile.GetComponent<RoadHandler>() != null)
            {
                WestOccupied = true;
                numberVerticalNearby++;
            }
            
        }
        

        if (tileOwner.eastTile != null)
        {
            if (tileOwner.eastTile.objectOnTile != null && tileOwner.eastTile.objectOnTile.GetComponent<RoadHandler>() != null)
            {
                EastOccupied = true;
                numberVerticalNearby++;
            }
           
        }

        //se c'è una o zero strade vicini mi evito il calcolo delle autostrade
        if (numberVerticalNearby >= 2)
        {
            if (NorthOccupied && EastOccupied)
            {
                if (tileOwner.north_eastTile.objectOnTile != null && tileOwner.north_eastTile.objectOnTile.GetComponent<RoadHandler>() != null)
                {
                    NorthEastOccupied = true;
                    numberDiagonalNearby++;
                }
            }

            if (NorthOccupied && WestOccupied)
            {
                if (tileOwner.north_westTile.objectOnTile != null && tileOwner.north_westTile.objectOnTile.GetComponent<RoadHandler>() != null)
                {
                    NorthWestOccupied = true;
                    numberDiagonalNearby++;
                }
            }

            if (SouthOccupied && EastOccupied)
            {
                if (tileOwner.south_eastTile.objectOnTile != null && tileOwner.south_eastTile.objectOnTile.GetComponent<RoadHandler>() != null)
                {
                    SouthEastOccupied = true;
                    numberDiagonalNearby++;
                }
            }

            if (SouthOccupied && WestOccupied)
            {
                if (tileOwner.south_westTile.objectOnTile != null && tileOwner.south_westTile.objectOnTile.GetComponent<RoadHandler>() != null)
                {
                    SouthWestOccupied = true;
                    numberDiagonalNearby++;
                }
            }

            if (numberDiagonalNearby > 0)
            {
                adjustType = RoadType.highway;

                //eliminazione delle strisce momentaneo per via del tentativo sotto
                foreach(GameObject g in verticalsLines)
                {
                    g.SetActive(false);
                }

                foreach (GameObject g in horizontalLines)
                {
                    g.SetActive(false);
                }
            }


            #region tentativo raggruppamento autostrada
            //tentativo di raggruppamento autostrada
            /*
            if(adjustType == RoadType.highway)
            {
                if (SouthOccupied && NorthOccupied)
                {
                    if (WestOccupied && EastOccupied)
                    {
                        //guardo gli altri
                        int numberOfVertical = 0;
                        int numberOfHorizontal = 0;

                        foreach(GestionalTile tile in tileOwner.tileNearbyList)
                        {
                            if (tile.objectOnTile!=null &&tile.objectOnTile.TryGetComponent<RoadHandler>(out RoadHandler road))
                            {
                                if(road.roadType == RoadType.highway)
                                {
                                    if(road.highwayDirection == HighwayDirection.Vertical)
                                    {
                                        numberOfVertical++;
                                    }
                                    else
                                    {
                                        numberOfHorizontal++;
                                    }
                                }
                            }
                        }

                        if(numberOfVertical > numberOfHorizontal)
                        {
                            highwayDirection = HighwayDirection.Vertical;

                            if (WestOccupied)
                            {
                                verticalLeftLines.SetActive(true);
                            }
                            else
                            {
                                verticalRightLines.SetActive(true);
                            }

                            foreach (GameObject g in verticalsLines)
                            {
                                g.SetActive(false);
                            }
                        }
                        else if(numberOfVertical < numberOfHorizontal)
                        {
                            highwayDirection = HighwayDirection.Horizontal;

                            if (NorthOccupied)
                            {
                                horizontalUpLines.SetActive(true);
                            }
                            else
                            {
                                horizontalDownLines.SetActive(true);
                            }

                            foreach (GameObject g in horizontalLines)
                            {
                                g.SetActive(false);
                            }
                        }
                        else
                        {
                            highwayDirection = HighwayDirection.None;
                        }
                               
                            
                    }
                    else
                    {
                        highwayDirection = HighwayDirection.Vertical;

                        if (WestOccupied)
                        {
                            verticalLeftLines.SetActive(true);
                        }
                        else
                        {
                            verticalRightLines.SetActive(true);
                        }

                        foreach(GameObject g in verticalsLines)
                        {
                            g.SetActive(false);
                        }
                    }
                }
                else if (WestOccupied && EastOccupied)
                {
                    highwayDirection = HighwayDirection.Horizontal;

                    if (NorthOccupied)
                    {
                        horizontalUpLines.SetActive(true);
                    }
                    else 
                    { 
                        horizontalDownLines.SetActive(true);
                    }

                    foreach(GameObject g in horizontalLines)
                    {
                        g.SetActive(false);
                    }
                }
                else
                {
                    highwayDirection = HighwayDirection.None;
                }
            }

            */
            #endregion

        }

        //check incrocio
        if (numberVerticalNearby==4 && numberDiagonalNearby == 0)
        {
            adjustType = RoadType.intersection;

            GestionalTile subtileSouth = tileOwner.subtiles[0, 0];
            GestionalTile subtileWest= tileOwner.subtiles[tileOwner.subtiles.GetLength(0)-1, 0];
            GestionalTile subtileNorth = tileOwner.subtiles[tileOwner.subtiles.GetLength(0)-1, tileOwner.subtiles.GetLength(1)-1];
            GestionalTile subtileEast = tileOwner.subtiles[0, tileOwner.subtiles.GetLength(1) - 1];
           
            //genero i semafori
            GameObject lightS= Instantiate(stopLight, subtileSouth.gameObject.transform.position, Quaternion.identity, subtileSouth.gameObject.transform);
            subtileSouth.SetObjectOnTile(lightS);
            lightS.transform.right = Vector3.left;

            GameObject lightW=Instantiate(stopLight, subtileWest.gameObject.transform.position, Quaternion.identity, subtileWest.gameObject.transform);
            subtileWest.SetObjectOnTile(lightW);
            lightW.transform.right = Vector3.back;

            GameObject lightN=Instantiate(stopLight, subtileNorth.gameObject.transform.position, Quaternion.identity, subtileNorth.gameObject.transform);
            subtileNorth.SetObjectOnTile(lightN);
            lightN.transform.right = Vector3.right;

            GameObject lightE=Instantiate(stopLight, subtileEast.gameObject.transform.position, Quaternion.identity, subtileEast.gameObject.transform);
            subtileEast.SetObjectOnTile(lightE);
            lightE.transform.right = Vector3.forward;

        }

        roadType = adjustType;

        GenerateObjectOnRoad(roadType);
        

       

    }

    private void GenerateObjectOnRoad(RoadType type)
    {
        switch (type)
        {
            case RoadType.normalRoad:
                foreach (GestionalTile tile in GetDecorationSpace())
                {
                    if (!tile.Occupied)
                    {
                        Vector3 tilePosition = tile.transform.position;

                        float objectNoise = Mathf.PerlinNoise(tilePosition.x * noiseScale, tilePosition.z * noiseScale);

                        if (objectNoise < decorationDensity)
                        {
                            GameObject decorationGameObject = Instantiate(normalRoadDecoration[UnityEngine.Random.Range(0, normalRoadDecoration.Count)], tile.transform);

                            tile.SetObjectOnTile(decorationGameObject);
                        }
                    }
                }
                break;

            case RoadType.intersection:
                foreach (GestionalTile tile in GetDecorationSpace())
                {
                    if (!tile.Occupied)
                    {
                        Vector3 tilePosition = tile.transform.position;

                        float objectNoise = Mathf.PerlinNoise(tilePosition.x * noiseScale, tilePosition.z * noiseScale);

                        if (objectNoise < decorationDensity)
                        {
                            GameObject decorationGameObject = Instantiate(normalRoadDecoration[UnityEngine.Random.Range(0, normalRoadDecoration.Count)], tile.transform);

                            tile.SetObjectOnTile(decorationGameObject);
                        }
                    }
                }
                break;
            case RoadType.highway:
                foreach (GestionalTile tile in GetDecorationSpace())
                {
                    if (!tile.Occupied)
                    {
                        Vector3 tilePosition = tile.transform.position;

                        float objectNoise = Mathf.PerlinNoise(tilePosition.x * noiseScale, tilePosition.z * noiseScale);

                        if (objectNoise < decorationDensity)
                        {
                            GameObject decorationGameObject = Instantiate(highWayDecoration[UnityEngine.Random.Range(0, highWayDecoration.Count)], tile.transform);

                            tile.SetObjectOnTile(decorationGameObject);
                        }
                    }
                }
                break;
        }

        

    }

    private List<GestionalTile> GetDecorationSpace()
    {
        List<GestionalTile> possibleDecorationTiles = new List<GestionalTile>(tileOwner.subTileList);

        if (!NorthOccupied)
        {
            for (int x = 0; x < tileOwner.subtiles.GetLength(0); x++)
            {
                possibleDecorationTiles.Remove(tileOwner.subtiles[x, tileOwner.subtiles.GetLength(0) - 1]);
            }
        }

        if (!SouthOccupied)
        {
            for (int x = 0; x < tileOwner.subtiles.GetLength(0); x++)
            {
                possibleDecorationTiles.Remove(tileOwner.subtiles[x, 0]);
            }
        }

        if (!EastOccupied)
        {
            for(int y = 0; y < tileOwner.subtiles.GetLength(1); y++)
            {
                possibleDecorationTiles.Remove(tileOwner.subtiles[tileOwner.subtiles.GetLength(1) - 1, y]);
            }
        }

        if (!WestOccupied)
        {
            for (int y = 0; y < tileOwner.subtiles.GetLength(1); y++)
            {
                possibleDecorationTiles.Remove(tileOwner.subtiles[0,y]);
            }
        }

        if(!SouthWestOccupied)
        {
            possibleDecorationTiles.Remove(tileOwner.subtiles[0, 0]);
        }

        if(!SouthEastOccupied)
        {
            possibleDecorationTiles.Remove(tileOwner.subtiles[0, tileOwner.subtiles.GetLength(1) - 1]);
        }

        if (!NorthWestOccupied)
        {
            possibleDecorationTiles.Remove(tileOwner.subtiles[tileOwner.subtiles.GetLength(0) - 1, 0]);
        }

        if (!NorthEastOccupied)
        {
            possibleDecorationTiles.Remove(tileOwner.subtiles[tileOwner.subtiles.GetLength(0) - 1, tileOwner.subtiles.GetLength(1) - 1]);
        }

        return possibleDecorationTiles;
    }

    private void ChangeNearbyTile(GestionalTile tile)
    {
        if(tile.objectOnTile.TryGetComponent<RoadHandler>(out RoadHandler road))
        {
            road.AdjustRoad();
        }
        else if(tile.objectOnTile.TryGetComponent<HouseHandler>(out HouseHandler house))
        {
            house.AdjustHouse();
        }
    }

    private void OnDestroy()
    {
        if (tileOwner != null)
        {
            tileOwner.Occupied = false;
            tileOwner.objectOnTile=null;

            foreach (GestionalTile tile in tileOwner.tileNearbyList)
            {
                if (tile.objectOnTile != null)
                {
                    ChangeNearbyTile(tile);
                }
            }

            //pulisco le decorazioni al ricalcolo della strada
            foreach (GestionalTile tile in tileOwner.subTileList)
            {
                if (tile.objectOnTile != null)
                {
                    Debug.Log($"Distruggo: {tile.objectOnTile}");
                    Destroy(tile.objectOnTile);
                    tile.Occupied = false;
                    tile.objectOnTile = null;                                        
                }
            }

            foreach(GestionalTile tile in tileOwner.subTileList)
            {
                tile.gestionalGridGenerator.GenerateEnviromentDecorations(tile);
            }
        }

    }

    

}
