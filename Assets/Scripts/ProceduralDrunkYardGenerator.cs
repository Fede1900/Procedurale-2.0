using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralDrunkYardGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> roomsPrefabs;
    [SerializeField] int roomCount;
    [SerializeField] float minRoomDistance;
    [SerializeField] Transform startPivot;
    [SerializeField] bool clearRoomOnNewGeneration;
    Vector3 _currentPosition;

    List<GameObject> _spawnedRooms=new();
    List<Vector3> _validPosition=new();
   
    

    private void Start()
    {
        if(clearRoomOnNewGeneration && _spawnedRooms.Count > 0)
        {
            ClearRoomSpawned();
        }

         GenerateRooms();
         PopulateRooms();
    }

   

    private void ClearRoomSpawned()
    {
        if (_spawnedRooms.Any())
        {
            foreach (var item in _spawnedRooms)
            {
                DestroyImmediate(item);
            }

            _spawnedRooms.Clear();
            _validPosition.Clear(); 
            _currentPosition = startPivot.transform.position;
        }
    }

    private void GenerateRooms()
    {
        if(_spawnedRooms == null)
        {
            _spawnedRooms= new List<GameObject>();
        }
        else if (clearRoomOnNewGeneration)
        {
            //TODO: Destroy immedate from editor script
            _spawnedRooms.Clear();
        }

        for(int i=0;i<roomCount;i++)
        {
            
            Vector3 validPosition = GetValidPosition();
            GameObject newRoom = Instantiate(roomsPrefabs[UnityEngine.Random.Range(0, roomsPrefabs.Count)],validPosition,Quaternion.identity);

            _spawnedRooms.Add(newRoom);
            _validPosition.Add(validPosition);
        }
    }

    private Vector3 GetValidPosition()
    {
        

        do
        {
            _currentPosition = GetDrunkyardPosition(_currentPosition,(int)minRoomDistance);
        }
        while (!IsPositionValid(_currentPosition));

        return _currentPosition;
    }

    public static Vector3 GetDrunkyardPosition(Vector3 currentPos,int minDistance)
    {
        Vector3 direction;
        float distance;

        do
        {
            direction = new Vector3(UnityEngine.Random.Range(-1, 2), 0, UnityEngine.Random.Range(-1, 2));
            distance = UnityEngine.Random.Range(-minDistance,minDistance);
        }
        while (direction.magnitude == 0);

        currentPos += direction * distance;
        return currentPos;
    }

    private bool IsPositionValid(Vector3 testPosition)
    {
        if (_validPosition.Contains(testPosition))
        {
            return false;
        }

        foreach(var position in _validPosition)
        {
            if(Vector3.Distance(position,testPosition)<minRoomDistance)
            {
                return false;
            }
        }

        return true;
    }
    public void PopulateRooms()
    {
        _spawnedRooms.Select(x => x.GetComponent<RoomHandler>()).ToList().ForEach(r => r.Initialize());
    }

#if UNITY_EDITOR
    public void GenerateRoomsFromEditor()
    {
        if (clearRoomOnNewGeneration)
        {
            ClearRoomSpawnedImmediate();
        }

        GenerateRooms();
        PopulateRooms();
    }

    private void ClearRoomSpawnedImmediate()
    {
        if (_spawnedRooms.Any())
        {
            foreach (var item in _spawnedRooms)
            {
                DestroyImmediate(item);
            }

            _spawnedRooms.Clear();
            _validPosition.Clear(); 
            _currentPosition = startPivot.transform.position;
        }
       

    }
#endif
}  
