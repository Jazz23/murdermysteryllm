using UnityEngine;
using UnityEditor;
using System.Collections;
using NavMeshPlus.Components;
using UnityEditor.Rendering.Universal;



[ExecuteInEditMode]
public class RoomGenerator : MonoBehaviour
{
    static private int ROW_ROOMS = 1;

    static private int COL_ROOMS = 2;

    [SerializeField]
    private float OFFSET = 100f;

    [SerializeField]
    private GameObject[] roomPrefabs = null;

    private Dictionary<(int, int), Room> roomCached = new Dictionary<(int, int), Room>();

    [SerializeField]
    private NavMeshSurface surface = null;

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 300, 90), "Room Generator");
        if (GUI.Button(new Rect(20, 40, 280, 20), "Generate Room"))
        {
            GenerateRoom();
        }

        if (GUI.Button(new Rect(20, 70, 280, 20), "Clear Room"))
        {
            ClearRoom();
        }
    }

    public void GenerateRoom()
    {
        for (int r = 0; r < ROW_ROOMS; r++)
        {
            for (int c = 0; c < COL_ROOMS; c++)
            {
                Room room = Instantiate(roomPrefabs[r * 3 + c], new Vector3(r * OFFSET, c * OFFSET, 0), Quaternion.identity).GetComponent<Room>();
                room.transform.parent = this.transform;
                room.name = room.GetRoomName();
                roomCached[(r, c)] = room;
                UpdateDoorWays(room, r, c); // Update the doorways based on the room's position
            }
        }

        ConnectDoorWays();

        // Build Nav Mesh Agent
        surface.BuildNavMeshAsync();
    }

    private string GenerateRoomName(Room room, int r, int c)
    {
        string name = $"{room.RoomType}";

        return name;
    }

    private void ConnectDoorWays()
    {

        string hoverMessage = "Go to";
        for (int r = 0; r < ROW_ROOMS; r++)
        {
            for (int c = 0; c < COL_ROOMS; c++)
            {
                Room room = roomCached[(r, c)];
                Door[] doors = room.GetComponentsInChildren<Door>();

                foreach (Door door in doors)
                {
                    switch (door.name)
                    {
                        case "North Door":
                            if (c >= COL_ROOMS - 1) break;
                            GameObject roomAbove = roomCached[(r, c + 1)].gameObject;
                            door.Location = roomAbove;
                            door.hoverMessage = hoverMessage + $" {roomAbove.name}?";
                            break;
                        case "South Door":
                            if (c <= 0) break;
                            GameObject roomBelow = roomCached[(r, c - 1)].gameObject;
                            door.Location = roomBelow;
                            door.hoverMessage = hoverMessage + $" {roomBelow.name}";
                            break;
                        case "West Door":
                            if (r <= 0) break;
                            GameObject roomLeft = roomCached[(r - 1, c)].gameObject;
                            door.Location = roomLeft;
                            door.hoverMessage = hoverMessage + $" {roomLeft.name}";
                            break;
                        case "East Door":
                            if (r >= ROW_ROOMS - 1) break;
                            GameObject roomRight = roomCached[(r + 1, c)].gameObject;
                            door.Location = roomRight;
                            door.hoverMessage = hoverMessage + $" {roomRight.name}";
                            break;
                    }
                }
            }
        }
    }

    private void UpdateDoorWays(Room room, int r, int c)
    {
        Door[] doors = room.Doors;
        for (int i = 0; i < doors.Length; i++)
        {
            switch (doors[i].name)
            {
                case "North Door":
                    doors[i].gameObject.SetActive(c < COL_ROOMS - 1);
                    break;
                case "South Door":
                    doors[i].gameObject.SetActive(c > 0);
                    break;
                case "West Door":
                    doors[i].gameObject.SetActive(r > 0);
                    break;
                case "East Door":
                    doors[i].gameObject.SetActive(r < ROW_ROOMS - 1);
                    break;
            }
        }
    }

    public void ClearRoom()
    {
#if UNITY_EDITOR
        foreach (var room in roomCached.Values)
            DestroyImmediate(room.gameObject);
#endif
        try
        {
            foreach (var room in roomCached.Values)
                Destroy(room.gameObject);
        }
        catch
        {
            Debug.LogError("Was Not able to Destroy Rooms");
        }

        surface.RemoveData();
        roomCached.Clear();
    }
}
