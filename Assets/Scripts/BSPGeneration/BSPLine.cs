using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BSPLine
{
    BSPOrientation _orientation;
    Vector2Int _coordimates;

    public BSPLine(BSPOrientation orientation, Vector2Int coordimates)
    {
        _orientation = orientation;
        _coordimates = coordimates;
    }

    public BSPOrientation Orientation { get { return _orientation; } }

    public Vector2Int Coordinaters { get { return _coordimates; } }
}

public enum BSPOrientation
{
    Horizontal=0, Vertical=1
}
