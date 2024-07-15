using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public GridSave gridSave= new GridSave();

    public void SaveToJson()
    {
        string gridSaveData=JsonUtility.ToJson(gridSave);
        string filePath = Application.persistentDataPath + "/GridSaveData.Json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, gridSaveData);
        Debug.Log("Saved");
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/GridSaveData.Json";
        string inventoryData=System.IO.File.ReadAllText(filePath);

        gridSave= JsonUtility.FromJson<GridSave>(inventoryData);
        Debug.Log("Loaded");
    }

    [Serializable]
    public class GridSave
    {
        public int GridWidth;
        public int GridHeight;
        public int TileCount;

        public List<TileSave> tiles;

        
    }

    [Serializable]
    public class TileSave
    {
        public bool Occupied;

        public string ObjectName="";

        public float objectRotationX = 0;
        public float objectRotationY = 0;
        public float objectRotationZ = 0;

        public List<innerTileSave> innerTileSaves;
    }

    [Serializable]
    public class innerTileSave
    {
        public bool Occupied;

        public string ObjectName = "";

        public float objectRotationX = 0;
        public float objectRotationY = 0;
        public float objectRotationZ = 0;
    }

    
}
