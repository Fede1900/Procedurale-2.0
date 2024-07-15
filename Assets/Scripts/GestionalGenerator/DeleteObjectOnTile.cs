using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObjectOnTile : MonoBehaviour
{
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.Log(hit);

                GestionalTile tile= hit.transform.gameObject.GetComponentInParent<GestionalTile>();

                if (tile!=null)
                {
                    if (tile.Occupied)
                    {
                        Destroy(tile.objectOnTile);

                    }
                }
            }
        }
    }
}
