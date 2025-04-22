using UnityEngine;


[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    

    [SerializeField]
    private RoomType roomType;

    public RoomType RoomType
    {
        get => roomType;
        set => roomType = value;
    }

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
    private GameObject[] items = null;

    public GameObject[] Items{
        get => items;
        set => items = value;
    }

    [SerializeField]
    private Transform[] spanZones = null;


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
		return this.roomType switch
		{
			RoomType.Hall => "HallWay",
			RoomType.Library => "Library",
			RoomType.BallRoom => "BallRoom",
            RoomType.BilliardRoom => "Billiard",
            RoomType.Conservatory => "Conservatory",
			RoomType.Kitchen => "Kitchen",
			RoomType.Study => "Study",
			RoomType.DiningRoom => "DiningRoom",
			RoomType.Lounge => "Lounge",
			_ => "Unknown Room Type",
		};
	}

    private void SpawnFurniture(){
        if (this.spanZones == null) { return; }

        if(this.items == null) { return; }

        for (int i = 0; i < this.items.Length; i++){
            // Randomly pick a unchcosen spanw zone 

            // place item in same positon
        }
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
