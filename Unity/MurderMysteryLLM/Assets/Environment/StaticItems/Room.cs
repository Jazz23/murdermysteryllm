using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    

    [SerializeField]
    private RoomType roomType;

    [SerializeField]
    private Door[] doors;

    public Door[] Doors
    {
        get => doors;
        set => doors = value;
    }


    [SerializeField]
    private Dictionary<Door, Room> neighbors = new Dictionary<Door, Room>();


    [SerializeField]
    private GameObject[] furniture = null;

    [SerializeField]
    private Transform spanZones = null;


    private void Awake()
    {
        
    }

    private void Update(){
        // handle destroyed furniture
    }



    private void OnSpawn(Room[] neighbors)
    {
        int numNeighbors = neighbors.Length;
        // if (numNeighbors == 0) return;
        doors = new Door[numNeighbors];
        for (int i = 0; i < numNeighbors; i++)
        {
            doors[i] = neighbors[i].GetComponent<Door>();
            doors[i].Location = neighbors[i].gameObject;
            string roomName = neighbors[i].GetRoomName();
            this.neighbors.Add(doors[i], neighbors[i]);

        }

    }

    private void OnDestroy()
    {

    }

    public string GetRoomName()
    {
        switch (this.roomType)
        {
            case RoomType.Hall:
                return "HallWay";
            case RoomType.Library:
                return "Library";
            case RoomType.BallRoom:
                return "BallRoom";
            case RoomType.Kitchen:
                return "Kitchen";
            case RoomType.Study:
                return "Study";
            case RoomType.DiningRoom:
                return "DiningRoom";
            case RoomType.Lounge:
                return "Lounge";
            default:
                return "Unknown Room Type";
        }
    }

    private void GenerateSpawn()
    {

    }

}

public enum RoomType
{
    BallRoom,
    BilliardRoom,
    Conservatory,
    DiningRoom,
    Hall,
    Kitchen,
    Library,
    Lounge,
    Study,
    Unknown
}
