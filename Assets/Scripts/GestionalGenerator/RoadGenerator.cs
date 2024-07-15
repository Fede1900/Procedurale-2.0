using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private LayerMask roadLayer;
    [SerializeField] private RoadHandler roadPrefab;

    private void FixedUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit,1000,roadLayer))
            {
                Debug.Log(hit);

                if (hit.transform.gameObject.TryGetComponent(out GestionalTile tile))
                {
                    
                    if (!tile.Occupied)
                    {
                        RoadHandler road= Instantiate(roadPrefab, tile.transform);
                        tile.SetObjectOnTile(road.gameObject);
                        //road.transform.localPosition=new Vector3(tile.SquareDimension/2,0,tile.SquareDimension/2);
                        
                        //tile.Occupied = true;
                        //tile.objectOnTile = road.gameObject;
                        //road.AdjustNearbyRoad();
                    }
                }
            }
        }
    }
}
