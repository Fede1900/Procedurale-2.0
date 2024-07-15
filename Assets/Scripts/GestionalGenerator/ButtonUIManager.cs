using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUIManager : MonoBehaviour
{
    [SerializeField] GameObject houseMode;
    [SerializeField] GameObject roadMode;
    [SerializeField] GameObject deleteMode;

    List<GameObject> buildmodes;

    private void Awake()
    {
        buildmodes = new List<GameObject>() { houseMode, roadMode, deleteMode};
    }

    public void SelectBuildMode(GameObject gameObject)
    {
        foreach (GameObject go in buildmodes)
        {
            go.SetActive(false);
        }

        gameObject.SetActive(true);
    }
}
