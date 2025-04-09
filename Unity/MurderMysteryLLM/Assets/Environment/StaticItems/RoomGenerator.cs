using UnityEditor.Rendering.Universal;
using UnityEngine;

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

    private void OnGUI() {
         GUI.Box(new Rect(10, 10, 300, 90), "Room Generator");
         if (GUI.Button(new Rect(20, 40, 280, 20), "Generate Room")) {
             GenerateRoom();
         }
            if (GUI.Button(new Rect(20, 70, 280, 20), "Clear Room")) {
                ClearRoom();
            }
        
        
    }

    private void GenerateRoom() {

        for (int r = 0; r < ROW_ROOMS; r++) {
            for (int c = 0; c < COL_ROOMS; c++){
				// create new room at (r,c) coordinate
                Room room = Instantiate(roomPrefab, new Vector3(r * OFFSET, c * OFFSET, 0), Quaternion.identity).GetComponent<Room>();
                
                room.transform.parent = this.transform;
                room.name = "Room_" + r + "_" + c;
                roomCached[room.name] = room;                
            }
        }
    }

    private void ClearRoom() {
        foreach (var room in roomCached.Values) {
            Destroy(room.gameObject);
        }
        roomCached.Clear();
    }

}
