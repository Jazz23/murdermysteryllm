using UnityEngine;
using UnityEditor;
using System.Collections;
using NavMeshPlus.Components;
using UnityEditor.Rendering.Universal;



[ExecuteInEditMode]
public class RoomGenerator : MonoBehaviour
{
    [SerializeField]
    private int ROW_ROOMS = 2;

    [SerializeField]
    private int COL_ROOMS = 3;

    [SerializeField]
    private float OFFSET = 100f;

    [SerializeField]
    private GameObject roomPrefab = null;

    private Dictionary<string, Room> roomCached = new Dictionary<string, Room>();

    [SerializeField]
    private NavMeshSurface surface= null;

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
    public void GenerateRoom() {
        for (int r = 0; r < ROW_ROOMS; r++) {
            for (int c = 0; c < COL_ROOMS; c++)
            {
                Room room = Instantiate(roomPrefab, new Vector3(r * OFFSET, c * OFFSET, 0), Quaternion.identity).GetComponent<Room>();
                room.transform.parent = this.transform;
                room.name = $"Room_{r}_{c}";
                roomCached[room.name] = room;

                UpdateDoorWays(room, r,c);
            }
        }

        
        // Build Nav Mesh Agent
        surface.BuildNavMeshAsync();
    }

    private void UpdateDoorWays(Room room, int r, int c)
    {
        Door[] doors = room.Doors;
        for (int i = 0; i < doors.Length; i++)
        {
            switch (doors[i].name)
            {
                case "North Door":
                    // North neighbor exists if c < COL_ROOMS - 1
                    doors[i].gameObject.SetActive(c < COL_ROOMS - 1);
                    break;
                case "South Door":
                    // South neighbor exists if c > 0
                    doors[i].gameObject.SetActive(c > 0);
                    break;
                case "West Door":
                    // West neighbor exists if r > 0
                    doors[i].gameObject.SetActive(r > 0);
                    break;
                case "East Door":
                    // East neighbor exists if r < ROW_ROOMS - 1
                    doors[i].gameObject.SetActive(r < ROW_ROOMS - 1);
                    break;
            }
        }
    }

    public void ClearRoom() {

# if UNITY_EDITOR
        foreach (var room in roomCached.Values) 
            DestroyImmediate(room.gameObject);
#endif
        try{
            foreach (var room in roomCached.Values)
                Destroy(room.gameObject);
        }
        catch {
            Debug.LogError("Was Not able to Destroy Rooms");
        }
        
        surface.RemoveData();
        roomCached.Clear();
    }

}
