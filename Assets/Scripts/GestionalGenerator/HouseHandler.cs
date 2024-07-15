using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HouseHandler : MonoBehaviour
{
    [Header("Normal road neighbour checker")]
    [SerializeField] bool _northOccupied;
    [SerializeField] bool _southOccupied;
    [SerializeField] bool _westOccupied;
    [SerializeField] bool _eastOccupied;

    [SerializeField] GestionalTile tileOwner;

    [SerializeField] List<RoadHandler> nearbyRoads;
    [SerializeField] RoadHandler facingRoad;

    bool placed;

    private void Awake()
    {
        tileOwner = GetComponentInParent<GestionalTile>();
        nearbyRoads= new List<RoadHandler>();

        

        
    }

    private void Start()
    {
        AdjustHouse();
    }

    public void AdjustHouse()
    {
        nearbyRoads.Clear();
        if (tileOwner.northTile != null)
        {
            if (tileOwner.northTile.objectOnTile != null && tileOwner.northTile.objectOnTile.TryGetComponent<RoadHandler>(out RoadHandler road))
            {
                if (road.roadType == RoadType.normalRoad)
                {
                    _northOccupied = true;
                    nearbyRoads.Add(road);
                }
                
               
            }

        }


        if (tileOwner.southTile != null)
        {
            if (tileOwner.southTile.objectOnTile != null && tileOwner.southTile.objectOnTile.TryGetComponent<RoadHandler>(out RoadHandler road))
            {
                if (road.roadType == RoadType.normalRoad)
                {
                    _southOccupied = true;
                    nearbyRoads.Add(road);
                }
                    
            }

        }


        if (tileOwner.westTile != null)
        {
            if (tileOwner.westTile.objectOnTile != null && tileOwner.westTile.objectOnTile.TryGetComponent<RoadHandler>(out RoadHandler road))
            {
                if (road.roadType == RoadType.normalRoad)
                {
                    _westOccupied = true;
                    nearbyRoads.Add(road);
                }
                    
            }

        }


        if (tileOwner.eastTile != null)
        {
            if (tileOwner.eastTile.objectOnTile != null && tileOwner.eastTile.objectOnTile.TryGetComponent<RoadHandler>(out RoadHandler road))
            {
                if (road.roadType == RoadType.normalRoad)
                {
                    _eastOccupied = true;
                    nearbyRoads.Add(road);
                }
                    
            }

        }

        if (nearbyRoads.Count==0)
        {
            Debug.LogError("nessuna strada disponibile per piazzare");
            Destroy(gameObject);
            return;
        }

        if (nearbyRoads.Contains(facingRoad))
        {
            //se la strada scelta precedentemente esiste ancora ed è sempre una strada normale lascio cosi com'è
            return;
        }

        RoadHandler selectedRoad = nearbyRoads[UnityEngine.Random.Range(0, nearbyRoads.Count)];

        Vector3 lookDirection=new Vector3(selectedRoad.transform.position.x,0,selectedRoad.transform.position.z);

        transform.rotation = Quaternion.identity;
        transform.LookAt(lookDirection);
        Debug.Log(lookDirection);

        facingRoad = selectedRoad;

        placed = true;

        //pulisco le decorazioni
        foreach (GestionalTile tile in tileOwner.subTileList)
        {
            if (tile.objectOnTile != null)
            {
                Destroy(tile.objectOnTile);
                tile.Occupied = false;
                tile.objectOnTile = null;
            }
        }
    }

    private void OnDestroy()
    {
        if (tileOwner != null)
        {
            tileOwner.Occupied = false;
            tileOwner.objectOnTile = null;

            if (placed)
            {
                //pulisco le decorazioni 
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

                foreach (GestionalTile tile in tileOwner.subTileList)
                {
                    tile.gestionalGridGenerator.GenerateEnviromentDecorations(tile);
                }
            }
            
        }
    }
}
