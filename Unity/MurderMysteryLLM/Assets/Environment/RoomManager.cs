using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    

    [SerializeField]
    private Room[] roomCache;

    [SerializeField]
    private int MAX_FURNITURE = 10;
    [SerializeField]
    private float red_herrings_coef = 0.5f ;

    
	void Awake()
	{
		roomCache= GameObject.FindObjectsOfType<Room>();
        for (int i=0; i<roomCache.Length; i++){
            Room room=roomCache[i];
            FillRoomWithItems(room, MAX_FURNITURE, red_herrings_coef);
        }
	}
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FillRoomWithItems(Room room, int max_items, float red_herrings){
        for (int i=0; i < max_items; i++){
            float rnd = Random.Range(0f, 1f);
             /// create clue
            if(rnd > red_herrings){
                // set clue is not red herring
            }else {
                // clue is red herring 
            }

            // add to room items
        }

    }
}
