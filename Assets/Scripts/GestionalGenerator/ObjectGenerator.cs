using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private List<GameObject> gameobjectsPrefabList;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000, objectLayer))
            {
                Debug.Log(hit);

                if (hit.transform.gameObject.TryGetComponent(out GestionalTile tile))
                {

                    if (!tile.Occupied)
                    {
                        GameObject road = Instantiate(gameobjectsPrefabList[UnityEngine.Random.Range(0,gameobjectsPrefabList.Count)], tile.transform);
                        tile.SetObjectOnTile(road);                        
                    }
                }
            }
        }
    }
}
